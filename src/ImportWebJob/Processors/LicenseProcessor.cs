// -----------------------------------------------------------------------
// <copyright file="LicenseProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Authentication;
    using Graph;
    using Storage;

    /// <summary>
    /// Provides the ability to query information pertaining to license based subscriptions.
    /// </summary>
    public class LicenseProcessor : IProcessor<SubscriptionEntity>
    {
        /// <summary>
        /// Name of the container where licensed based subscription information is stored.
        /// </summary>
        private const string LicenseContainerName = "license";

        /// <summary>
        /// Partition key for the license SKUs storage table.
        /// </summary>
        private const string SkuPartitionKey = "LicenseSku";

        /// <summary>
        /// Name of the storage table where license SKU information is stored.
        /// </summary>
        private const string SkuTableName = "licenseskus";

        /// <summary>
        /// Provides the ability to manage access tokens.
        /// </summary>
        private ITokenManagement tokenMgmt;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseProcessor"/> class.
        /// </summary>
        public LicenseProcessor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseProcessor"/> class.
        /// </summary>
        /// <param name="tokenMgmt">Provides the ability to manage access tokens.</param>
        public LicenseProcessor(ITokenManagement tokenMgmt)
        {
            tokenMgmt.AssertNotNull(nameof(tokenMgmt));

            this.tokenMgmt = tokenMgmt;
        }

        /// <summary>
        /// Gets a reference to the token management implementation.
        /// </summary>
        private ITokenManagement TokenMgmt => this.tokenMgmt ?? (this.tokenMgmt = new TokenManagement());

        /// <summary>
        /// Processes the license details for the specified <see cref="SubscriptionEntity"/>.
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
            IGraphServiceClient client;
            IGraphServiceSubscribedSkusCollectionPage skus;
            List<LicenseSkuEntity> skuEntities;

            try
            {
                // Verify that the subscription billing type is set to license. Subscriptions that have other 
                // billing types configured will not have Azure usage records. Since that is the case an 
                // attempt to query for any usage reocrds will fail. 
                if (!entity.BillingType.Equals("license", StringComparison.CurrentCulture)
                    && entity.Status.Equals("deleted", StringComparison.CurrentCulture))
                {
                    return;
                }

                client = new GraphServiceClient(new AuthenticationProvider(this.TokenMgmt, entity.CustomerId));
                skuEntities = new List<LicenseSkuEntity>();
                skus = await client.SubscribedSkus.Request().GetAsync();

                skuEntities.AddRange(skus.Select(s => new LicenseSkuEntity
                {
                    ActiveUnits = s.PrepaidUnits.Enabled.Value,
                    AppliesTo = s.AppliesTo,
                    AvailableUnits = s.PrepaidUnits.Enabled.Value + s.PrepaidUnits.Warning.Value - s.ConsumedUnits.Value,
                    CapabilityStatus = s.CapabilityStatus,
                    ConsumedUnits = s.ConsumedUnits.Value,
                    CustomerId = entity.CustomerId,
                    LicenseSkuCaptureDate = DateTime.Now,
                    PartitionKey = $"{DateTime.Now:yyyy-MM-dd}",
                    RowKey = $"{entity.CustomerId}_{s.SkuId}",
                    SkuId = s.SkuId.Value.ToString(),
                    SkuPartNumber = s.SkuPartNumber,
                    SuspendedUnits = s.PrepaidUnits.Suspended.Value,
                    TotalUnits = s.PrepaidUnits.Enabled.Value + s.PrepaidUnits.Warning.Value,
                    WarningUnits = s.PrepaidUnits.Warning.Value
                }));

                if (skuEntities.Count > 0)
                {
                    await Program.Core.Storage.WriteToBlobAsync(
                        LicenseContainerName, $"{entity.SubscriptionId}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json", skuEntities);

                    Parallel.ForEach(skuEntities, async sku =>
                    {
                        // Write the usage record to an Azure storage table.
                        await Program.Core.Storage.WriteToTableAsync(SkuTableName, sku);
                    });
                }
            }
            finally
            {
                client = null;
                skuEntities = null;
                skus = null;
            }
        }
    }
}