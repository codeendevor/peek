// -----------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Provides quick access to configurations stored in different places such as app.config
    /// </summary>
    public static class ApplicationConfiguration
    {
        /// <summary>
        /// Gets the Azure Active Directory endpoint.
        /// </summary>
        public static string ActiveDirectoryEndpoint
            => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public static string AppInsightsInstrumentationKey
            => ConfigurationManager.AppSettings["AppInsights.InstrumentationKey"];

        /// <summary>
        /// Gets the Azure Resource Manager API endpoint address.
        /// </summary>
        public static string AzureResourceManagerEndpoint
            => ConfigurationManager.AppSettings["AzureResourceManagerEndpoint"];

        /// <summary>
        /// Gets the Direct AAD application identifier.
        /// </summary>
        public static string DirectApplicationId
            => ConfigurationManager.AppSettings["Direct.ApplicationId"];

        /// <summary>
        /// Gets the Direct AAD application secret.
        /// </summary>
        public static string DirectApplicationSecret
            => ConfigurationManager.AppSettings["Direct.ApplicationSecret"];

        /// <summary>
        /// Gets the Direct AAD application tenant identifier.
        /// </summary>
        public static string DirectApplicationTenantId
            => ConfigurationManager.AppSettings["Direct.ApplicationTenantId"];

        /// <summary>
        /// Gets the Microsoft Graph endpoint address.
        /// </summary>
        public static string GraphEndpoint
            => ConfigurationManager.AppSettings["MicrosoftGraphEndpoint"];

        /// <summary>
        /// Gets a boolean indicating if development mode is enabled or not.
        /// </summary>
        public static bool IsDevelopment
            => Convert.ToBoolean(ConfigurationManager.AppSettings["IsDevelopment"]);

        /// <summary>
        /// Gets the Partner Center Azure AD application identifier.
        /// </summary>
        public static string PartnerCenterApplicationId
            => ConfigurationManager.AppSettings["PartnerCenter.ApplicationId"];

        /// <summary>
        /// Gets the Partner Center Azure AD application secret.
        /// </summary>
        public static string PartnerCenterApplicationSecret
            => ConfigurationManager.AppSettings["PartnerCenter.ApplicationSecret"];

        /// <summary>
        /// Gets the Partner Center resource URL.
        /// </summary>
        public static string PartnerCenterEndpoint
            => ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

        /// <summary>
        /// Gets the tenant identifier that owns the Partner Center Azure AD application.
        /// </summary>
        public static string PartnerCenterTenantId
            => ConfigurationManager.AppSettings["PartnerCenter.ApplicationTenantId"];

        /// <summary>
        /// Gets the Peek Azure AD application identifier.
        /// </summary>
        public static string PeekApplicationId
            => ConfigurationManager.AppSettings["Peek.ApplicationId"];

        /// <summary>
        /// Gets the Peek Azure AD application secret.
        /// </summary>
        public static string PeekApplicationSecret
            => ConfigurationManager.AppSettings["Peek.ApplicationSecret"];

        /// <summary>
        /// Gets the tenant identifier that owns the Peek Azure AD application.
        /// </summary>
        public static string PeekTenantId
            => ConfigurationManager.AppSettings["Peek.ApplicationTenantId"];

        /// <summary>
        /// Gets the queue batch size; this is the number of messages that will be retrieved from the queue at once.
        /// </summary>
        public static int QueueBatchSize
            => Convert.ToInt32(ConfigurationManager.AppSettings["Storage.QueueBatchSize"]);

        /// <summary>
        /// Gets the subscription queue name.
        /// </summary>
        public static string SubscriptionsQueueName
            => ConfigurationManager.AppSettings["Storage.SubscriptionsQueueName"];

        /// <summary>
        /// Gets the number of days for which usage will be requested.
        /// </summary>
        public static int UsageRequestNumberOfDays
            => Convert.ToInt32(ConfigurationManager.AppSettings["Usage.RequestNumberOfDays"]);
    }
}