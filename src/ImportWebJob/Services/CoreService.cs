// -----------------------------------------------------------------------
// <copyright file="CoreService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Services
{
    using Authentication;
    using Azure.Commerce.UsageAggregates;
    using Configuration;
    using Store.PartnerCenter;
    using Telemetry;

    /// <summary>
    /// Provides quick access to core services utilized by this application.
    /// </summary>
    public class CoreService : ICoreService
    {
        /// <summary>
        /// Provides the ability to query usage records for direct Azure subscriptions.
        /// </summary>
        private static IUsageAggregationManagementClient aggregation;

        /// <summary>
        /// Provides the ability to interact with the Partner Center API.
        /// </summary>
        private static IAggregatePartner partnerCenter;

        /// <summary>
        /// Provides the ability to interact with an Azure storage account.
        /// </summary>
        private static IStorageService storage;

        /// <summary>
        /// Provides the ability to log telemetry for the application.
        /// </summary>
        private static ITelemetryProvider telemetry;

        /// <summary>
        /// Gets a reference to the aggregation management client.
        /// </summary>
        /// <remarks>
        /// This is utilized to obtain usage records for subscriptions obtained directly from Microsoft.
        /// </remarks>
        public IUsageAggregationManagementClient AggrationManagement => aggregation ?? (aggregation = new UsageAggregationManagementClient());

        /// <summary>
        /// Gets a reference to the Partner Center service. 
        /// </summary>
        public IAggregatePartner PartnerCenter
        {
            get
            {
                ITokenManagement tokenMgmt;

                if (partnerCenter != null)
                {
                    return partnerCenter;
                }

                try
                {
                    tokenMgmt = new TokenManagement();

                    partnerCenter = PartnerService.Instance.CreatePartnerOperations(
                        tokenMgmt.GetPartnerCenterAppOnlyCredentials(
                            $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.PartnerCenterTenantId}"));

                    return partnerCenter;
                }
                finally
                {
                    tokenMgmt = null;
                }
            }
        }

        /// <summary>
        /// Gets a reference to the storage service.
        /// </summary>
        public IStorageService Storage => storage ?? (storage = new StorageService());

        /// <summary>
        /// Gets a reference to the the telemetry provider.
        /// </summary>
        /// <remarks>
        /// This is utilized to capture diagnostic information for the application.
        /// </remarks>
        public ITelemetryProvider Telemetry
        {
            get
            {
                if (telemetry != null)
                {
                    return telemetry;
                }

                if (string.IsNullOrEmpty(ApplicationConfiguration.AppInsightsInstrumentationKey))
                {
                    telemetry = new EmptyTelemetryProvider();
                }
                else
                {
                    telemetry = new ApplicationInsightsTelemetryProvider();
                }

                return telemetry;
            }
        }
    }
}