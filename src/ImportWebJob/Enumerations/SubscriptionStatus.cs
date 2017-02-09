// -----------------------------------------------------------------------
// <copyright file="SubscriptionStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Peek.ImportWebJob.Enumerations
{
    /// <summary>
    /// Lists the available states for a subscription.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// Indicates nothing - no status, used as an initializer.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Indicates that the subscription is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Indicates that the subscription has been suspended.
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// Indicates that the subscription has been deleted.
        /// </summary>
        Deleted = 3
    }
}
