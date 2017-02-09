// -----------------------------------------------------------------------
// <copyright file="UsageProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors.PartnerCenter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Newtonsoft.Json;
    using Storage;
    using Store.PartnerCenter.Enumerators;
    using Store.PartnerCenter.Exceptions;
    using Store.PartnerCenter.Models;
    using Store.PartnerCenter.Models.RateCards;
    using Store.PartnerCenter.Models.Utilizations;

    /// <summary>
    /// Azure usage record processor for a subscription obtained using Partner Center.
    /// </summary>
    public class UsageProcessor : IProcessor<SubscriptionEntity>
    {
        /// <summary>
        /// Name of the table where Azure usage records will be stored.
        /// </summary>
        private const string AzureUsageTableName = "azureusage";

        /// <summary>
        /// Name of the container where usage records will be stored.
        /// </summary>
        private const string UsageContainerName = "usage";

        /// <summary>
        /// Provides access to rate card information.
        /// </summary>
        private static AzureRateCard rateCard;

        /// <summary>
        /// Gets a reference to the Azure rate card information.
        /// </summary>
        private static AzureRateCard RateCard
            => rateCard ?? (rateCard = Program.Core.PartnerCenter.RateCards.Azure.Get());

        /// <summary>
        /// Processes the usage records for the specified <see cref="SubscriptionEntity"/>.
        /// </summary>
        /// <param name="entity">An aptly populated instance of <see cref="SubscriptionEntity"/>.</param>
        /// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Parallel.ForEach is formatted for readability.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Parallel.ForEach is formatted for readability.")]
        public async Task ProcessAsync(SubscriptionEntity entity)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            IResourceCollectionEnumerator<ResourceCollection<AzureUtilizationRecord>> recordsEnumerator;
            List<UsageRecordEntity> usage;
            ResourceCollection<AzureUtilizationRecord> records;

            entity.AssertNotNull(nameof(entity));

            try
            {
                // Capture the start time for diagnostic purposes.
                startTime = DateTime.Now;

                // Verify that the subscription billing type is set to usage. Subscriptions that have other 
                // billing types configured will not have Azure usage records. Since that is the case an 
                // attempt to query for any usage reocrds will fail. 
                if (!entity.BillingType.Equals("usage", StringComparison.CurrentCulture)
                    && entity.Status.Equals("deleted", StringComparison.CurrentCulture))
                {
                    return;
                }

                // Capture the event properties for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", entity.CustomerId },
                    { "SubscriptionId", entity.SubscriptionId }
                };

                usage = new List<UsageRecordEntity>();

                records = await Program.Core.PartnerCenter
                    .Customers.ById(entity.CustomerId).Subscriptions.ById(entity.SubscriptionId)
                    .Utilization.Azure.QueryAsync(
                        DateTime.Now.AddDays(-ApplicationConfiguration.UsageRequestNumberOfDays), DateTime.Now, size: 500);

                recordsEnumerator = Program.Core.PartnerCenter.Enumerators.Utilization.Azure.Create(records);

                while (recordsEnumerator != null && recordsEnumerator.HasValue)
                {
                    usage.AddRange(recordsEnumerator.Current.Items.Select(record => new UsageRecordEntity
                    {
                        Category = record.Resource.Category,
                        CustomerId = entity.CustomerId,
                        Id = record.Resource.Id,
                        Location = record.InstanceData?.Location,
                        Name = record.Resource.Name,
                        Quantity = Convert.ToDouble(record.Quantity),
                        ResourceUri = record.InstanceData?.ResourceUri.ToString(),
                        Subcategory = record.Resource.Subcategory,
                        SubscriptionId = entity.SubscriptionId,
                        Tags = JsonConvert.SerializeObject(record.InstanceData?.Tags),
                        Total = this.GetTotal(record.Resource.Id, record.Quantity),
                        UniqueId = Guid.NewGuid(),
                        Unit = record.Unit,
                        UsageEndTime = record.UsageEndTime,
                        UsageStartTime = record.UsageStartTime
                    }));

                    await recordsEnumerator.NextAsync();
                }

                // Track the event metrics for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfUsageRecords", usage.Count }
                };

                Program.Core.Telemetry.TrackEvent("ProcessAsync", eventProperties, eventMetrics);

                if (usage.Count > 0)
                {
                    await Program.Core.Storage.WriteToBlobAsync(
                        UsageContainerName, $"{entity.SubscriptionId}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json", usage);

                    Parallel.ForEach(usage, async record =>
                    {
                        // Ensure the partition key for the entity is configured.
                        record.PartitionKey = $"{record.CustomerId}_{record.SubscriptionId}";

                        // Ensure the row key for the entity is configured.
                        record.RowKey = $"{record.UniqueId}";

                        // Write the usage record to an Azure storage table.
                        await Program.Core.Storage.WriteToTableAsync(AzureUsageTableName, record);
                    });
                }
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                records = null;
                recordsEnumerator = null;
                usage = null;
            }
        }

        /// <summary>
        /// Calculates the total for the specified resource.
        /// </summary>
        /// <param name="resourceId">Identifier of the resource.</param>
        /// <param name="quantity">Quantity of the resource consumed.</param>
        /// <returns>The total billable amount for the consumed resource.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="resourceId"/> is null.
        /// </exception>
        private double GetTotal(string resourceId, decimal quantity)
        {
            AzureMeter meter;
            decimal billableQuantity;
            decimal rate = 0;

            resourceId.AssertNotEmpty(nameof(resourceId));

            try
            {
                // Obtain a reference to the metere associated with the usage record. 
                meter = RateCard.Meters.Single(x => x.Id.Equals(resourceId, StringComparison.CurrentCultureIgnoreCase));

                // Calculate the billable quantity by subtracting the including quantity from the included quantity.
                billableQuantity = quantity - meter.IncludedQuantity;

                if (billableQuantity > 0)
                {
                    // Obtain the rate for the quantity consumed. The following LINQ statement will select the rate
                    // based upon the billable quantity and corresponding tier price (if there is tier pircing). 
                    rate = meter.Rates
                        .Where(x => x.Key <= billableQuantity).Select(x => x.Value).Last();
                }

                return Convert.ToDouble(billableQuantity * rate);
            }
            catch (InvalidOperationException ex)
            {
                Program.Core.Telemetry.TrackException(ex, new Dictionary<string, string> { { "ResourceId", resourceId } });
                return 0;
            }
            finally
            {
                meter = null;
            }
        }
    }
}