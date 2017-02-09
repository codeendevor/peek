// -----------------------------------------------------------------------
// <copyright file="SubscriptionEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Storage
{
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a subscription that is stored in a storage table.
    /// </summary>
    public class SubscriptionEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the way billing is processed for the subscription. 
        /// </summary>
        /// <remarks>
        /// If this property is set to anything other than usage that will 
        /// indicate the subscription is not an Azure subscription.
        /// </remarks>
        public string BillingType { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the custom that owns the subscription.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the friendly name assigned to the subscription.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the provider for the subscription.
        /// </summary>
        /// <remarks>
        /// This property can have one of three values. It will be set to Direct when 
        /// the subscription was purchase directly from Microsoft. It will be set to 
        /// EnterpriseAgreement when the subscription was obtained as part of an Enterprise
        /// Agreement. Finally, it will be set to Partner when the subscription was obtained
        /// from a Cloud Solution Provider (CSP) partner.
        /// </remarks>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the status for the subscription.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the identifier assigned to the subscription.
        /// </summary>
        public string SubscriptionId { get; set; }
    }
}
