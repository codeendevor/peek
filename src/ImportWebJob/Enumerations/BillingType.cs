// -----------------------------------------------------------------------
// <copyright file="BillingType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Enumerations
{
    /// <summary>
    /// The way billing is processed for a <see cref="Storage.SubscriptionEntity"/>.
    /// </summary>
    public enum BillingType
    {
        /// <summary>
        /// Indicates nothing - not, and is used as an initializer.
        /// </summary>
        None = 0,

        /// <summary>
        /// Usages based billing (Azure subscriptions)
        /// </summary>
        Usage = 1,

        /// <summary>
        /// License based billing (Office 365 subscriptions)
        /// </summary>
        License = 2
    }
}