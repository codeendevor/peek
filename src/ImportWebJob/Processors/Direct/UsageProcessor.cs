// -----------------------------------------------------------------------
// <copyright file="UsageProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors.Direct
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Authentication;
    using Azure;
    using Azure.Commerce.UsageAggregates;
    using Azure.Commerce.UsageAggregates.Models;
    using Configuration;
    using IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;
    using Storage;

    /// <summary>
    /// Azure usage record processor for a subscription obtained directly from Microsoft.
    /// </summary>
    public class UsageProcessor : IProcessor<SubscriptionEntity>
    {
        /// <summary>
        /// Used to obtain usage records for the specified subscription.
        /// </summary>
        private IUsageAggregationManagementClient aggregation;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageProcessor"/> class.
        /// </summary>
        public UsageProcessor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageProcessor"/> class.
        /// </summary>
        /// <param name="client">The management client for retrieving usage records.</param>
        public UsageProcessor(IUsageAggregationManagementClient client)
        {
            this.aggregation = client;
        }

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
            List<UsageRecordEntity> records;
            UsageAggregationGetResponse usage;

            try
            {
                if (this.aggregation == null)
                {
                    this.aggregation = await this.GetAggregationManagementClient(entity.SubscriptionId);
                }

                records = new List<UsageRecordEntity>();

                // TODO - Refactor this code to utillize the continuation token if one is returned from this call.
                usage = await this.aggregation.UsageAggregates.GetAsync(
                    DateTime.Now.AddDays(-ApplicationConfiguration.UsageRequestNumberOfDays),
                    DateTime.Now,
                    AggregationGranularity.Daily,
                    true,
                    string.Empty);

                records.AddRange(usage.UsageAggregations.Select(r => new UsageRecordEntity
                {
                    Category = r.Properties.MeterCategory,
                    CustomerId = entity.CustomerId,
                    Id = r.Properties.MeterId,
                    Location = InstanceData.GetInstance(r.Properties.InstanceData).Location,
                    Name = r.Name,
                    Quantity = Convert.ToDouble(r.Properties.Quantity),
                    ResourceUri = InstanceData.GetInstance(r.Properties.InstanceData).ResourceUri.ToString(),
                    Subcategory = r.Properties.MeterSubCategory,
                    SubscriptionId = entity.SubscriptionId,
                    Tags = JsonConvert.SerializeObject(InstanceData.GetInstance(r.Properties.InstanceData)?.Tags),
                    Total = 0, // GetTotal(record.Resource.Id, record.Quantity),
                    UniqueId = Guid.NewGuid(),
                    Unit = r.Properties.Unit,
                    UsageEndTime = r.Properties.UsageEndTime,
                    UsageStartTime = r.Properties.UsageStartTime
                }));

                if (records.Count > 0)
                {
                    await Program.Core.Storage.WriteToBlobAsync(
                        "usage", $"{entity.SubscriptionId}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json", records);

                    Parallel.ForEach(records, async record =>
                    {
                        // Ensure the partition key for the entity is configured.
                        record.PartitionKey = $"{record.CustomerId}_{record.SubscriptionId}";

                        // Ensure the row key for the entity is configured.
                        record.RowKey = $"{record.UniqueId}";

                        // Write the usage record to an Azure storage table.
                        await Program.Core.Storage.WriteToTableAsync("azureusage", record);
                    });
                }
            }
            finally
            {
                records = null;
                usage = null;
            }
        }

        /// <summary>
        /// Gets reference to the <see cref="UsageAggregationManagementClient"/> bound to the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <returns>An aptly configured instance of the <see cref="UsageAggregationManagementClient"/> class.</returns>
        private async Task<IUsageAggregationManagementClient> GetAggregationManagementClient(string subscriptionId)
        {
            AuthenticationResult token;
            ITokenManagement tokenMgmt;
            TokenCloudCredentials credentials;

            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                tokenMgmt = new TokenManagement();

                token = await tokenMgmt.GetAppOnlyTokenAsync(
                    $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.DirectApplicationTenantId}",
                     ApplicationConfiguration.DirectApplicationId,
                     ApplicationConfiguration.DirectApplicationSecret,
                     ApplicationConfiguration.AzureResourceManagerEndpoint);

                credentials = new TokenCloudCredentials(subscriptionId, token.AccessToken);

                return new UsageAggregationManagementClient(credentials);
            }
            finally
            {
                token = null;
                tokenMgmt = null;
            }
        }
    }
}