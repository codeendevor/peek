// -----------------------------------------------------------------------
// <copyright file="LicenseSkuEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Storage
{
    using System;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a subscribed product owned by a customer that is stored in a storage table.
    /// </summary>
    public class LicenseSkuEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the number of units available for assignment.
        /// </summary>
        public int AvailableUnits { get; set; }

        /// <summary>
        /// Gets or sets the number of active units.
        /// </summary>
        public int ActiveUnits { get; set; }

        /// <summary>
        /// Gets or sets the entity that can utilize this SKU.
        /// </summary>
        public string AppliesTo { get; set; }

        /// <summary>
        /// Gets or sets the SKU status of a product.
        /// </summary>
        public string CapabilityStatus { get; set; }

        /// <summary>
        /// Gets or sets the number of consumed units.
        /// </summary>
        public int ConsumedUnits { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier that has subscribed to this SKU.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        ///  Gets or sets the date for when this SKU was captured.
        /// </summary>
        public DateTime LicenseSkuCaptureDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the SKU.
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// Gets or sets the SKU part number.
        /// </summary>
        public string SkuPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of suspended units.
        /// </summary>
        public int SuspendedUnits { get; set; }

        /// <summary>
        /// Gets or sets the total units, which is the sum of active and warning units.
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// Gets or sets the number of warning units.
        /// </summary>
        public int WarningUnits { get; set; }
    }
}