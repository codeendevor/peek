// -----------------------------------------------------------------------
// <copyright file="Functions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Configuration;
    using Newtonsoft.Json;
    using Storage;
    using Store.PartnerCenter.Exceptions;

    /// <summary>
    /// Provides definition for all of the available Azure WebJob functions.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Daily job that is triggered everyday at 2:00 AM.  
        /// </summary>
        /// <param name="timer">An instance of the <see cref="TimerInfo"/> class.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Parallel.ForEach is formatted for readability.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Parallel.ForEach is formatted for readability.")]
        public static void DailyCronJob([TimerTrigger("0 0 2 * * *")] TimerInfo timer)
        {
            if (ApplicationConfiguration.IsDevelopment)
            {
                Program.Core.Storage.DevelopmentModeCleanup();
            }

            try
            {
                Parallel.ForEach(Program.CustomerProcessors, async processor =>
                {
                    await processor.ProcessAsync();
                });
            }
            catch (Exception ex)
            {
                Program.Core.Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Processes a subscription once it has been written to the Azure Storage Queue named Subscriptions.
        /// </summary>
        /// <param name="subscription">A JSON representation of a <see cref="SubscriptionEntity"/>.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public static async Task ProcessSubscriptionAsync([QueueTrigger("subscriptions")]string subscription)
        {
            SubscriptionEntity s;

            subscription.AssertNotEmpty(nameof(subscription));

            try
            {
                s = JsonConvert.DeserializeObject<SubscriptionEntity>(subscription);

                if (s.BillingType.Equals("license", StringComparison.CurrentCultureIgnoreCase))
                {
                    await Program.LicenseProcessor.ProcessAsync(s);
                }
                else
                {
                    await Program.UsageProcessors[s.Provider].Invoke(s);
                }
            }
            catch (PartnerException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Program.Core.Telemetry.TrackException(ex);
            }
            finally
            {
                s = null;
            }
        }
    }
}