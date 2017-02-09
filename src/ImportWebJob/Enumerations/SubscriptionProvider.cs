// -----------------------------------------------------------------------
// <copyright file="SubscriptionProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Enumerations
{
    /// <summary>
    /// Specifies the provider for a <see cref="Storage.SubscriptionEntity"/>.
    /// </summary>
    public enum SubscriptionProvider
    {
        /// <summary>
        /// A <see cref="Storage.SubscriptionEntity"/> that has been established directly with Microsoft.
        /// </summary>
        Direct,

        /// <summary>
        /// A <see cref="Storage.SubscriptionEntity"/> that has been established through the Cloud Solution Provider (CSP) program.
        /// </summary>
        PartnerCenter
    }
}