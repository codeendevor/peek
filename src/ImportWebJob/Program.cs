// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Configuration;
    using Processors;
    using Services;
    using Storage;

    /// <summary>
    /// Defines the core objects and properties for the WebJob.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets a reference to the core service provider.
        /// </summary>
        public static ICoreService Core { get; private set; }

        /// <summary>
        /// Gets a list of the available customer processors.
        /// </summary>
        public static List<IProcessor> CustomerProcessors { get; private set; }

        /// <summary>
        /// Gets the licensed based subscription processor.
        /// </summary>
        public static IProcessor<SubscriptionEntity> LicenseProcessor { get; private set; }

        /// <summary>
        /// Gets a dictionary of available usage processors.
        /// </summary>
        public static Dictionary<string, Func<SubscriptionEntity, Task>> UsageProcessors { get; private set; }

        /// <summary>
        /// Entry point for the WebJob.
        /// </summary>
        internal static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            Core = new CoreService();

            if (!string.IsNullOrEmpty(ApplicationConfiguration.AppInsightsInstrumentationKey))
            {
                ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
                    ApplicationConfiguration.AppInsightsInstrumentationKey;
            }

            CustomerProcessors = GetAvailableCustomerProcessors();

            LicenseProcessor = new LicenseProcessor();

            UsageProcessors = new Dictionary<string, Func<SubscriptionEntity, Task>>()
            {
                { "Direct", (subscription) => new Processors.Direct.UsageProcessor().ProcessAsync(subscription) },
                { "PartnerCenter", (subscription) => new Processors.PartnerCenter.UsageProcessor().ProcessAsync(subscription) }
            };

            // Configure the number of messages to retrieve from the queue at once. 
            config.Queues.BatchSize = ApplicationConfiguration.QueueBatchSize;

            // Enable the use of the timer extensions. 
            config.UseTimers();

            var host = new JobHost(config);
            host.RunAndBlock();
        }

        /// <summary>
        /// Gets a list of the available customer processors.
        /// </summary>
        /// <returns>A list of the available processors.</returns>
        private static List<IProcessor> GetAvailableCustomerProcessors()
        {
            List<IProcessor> processors = new List<IProcessor>();

            if (!string.IsNullOrEmpty(ApplicationConfiguration.DirectApplicationId) &&
                !string.IsNullOrEmpty(ApplicationConfiguration.DirectApplicationSecret) &&
                !string.IsNullOrEmpty(ApplicationConfiguration.DirectApplicationTenantId))
            {
                processors.Add(new Processors.Direct.CustomerProcessor());
            }

            if (!string.IsNullOrEmpty(ApplicationConfiguration.PartnerCenterApplicationId) &&
                !string.IsNullOrEmpty(ApplicationConfiguration.PartnerCenterApplicationSecret) &&
                !string.IsNullOrEmpty(ApplicationConfiguration.PartnerCenterTenantId))
            {
                processors.Add(new Processors.PartnerCenter.CustomerProcessor());
            }

            return processors;
        }
    }
}