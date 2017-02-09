// -----------------------------------------------------------------------
// <copyright file="UsageRecordEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Storage
{
    using System;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// An Azure usage records for a consumed service.
    /// </summary>
    public class UsageRecordEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageRecordEntity"/> class.
        /// </summary>
        public UsageRecordEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageRecordEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">Partition key for the entity.</param>
        /// <param name="rowKey">Row key for the entity.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="partitionKey"/> is null.
        /// or
        /// <paramref name="rowKey"/> is null.
        /// </exception>
        public UsageRecordEntity(string partitionKey, string rowKey)
        {
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the category of the consumed Azure resource.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the customer that owns the usage record.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the Azure resource that was consumed.
        /// Also known as the resourceID or resourceGUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the region were the resource was consumed.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the Azure resource being consumed.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity consumed of the Azure resource.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the region for the consumed Azure resource.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified Azure resource ID, which includes the 
        /// resource groups and the instance name.
        /// </summary>
        public string ResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the sub-category of the consumed Azure resource.
        /// </summary>
        public string Subcategory { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the subscription that owns the usage record.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the the resource tags specified for the resource.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the billable total for the resource(s) consumed.
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the record.
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the type of quantity (hours, bytes, etc...).
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the end of the usage aggregation time range. The response is grouped
        /// by the time of consumption (when the resource was actually used VS. when was
        /// it reported to the billing system).
        /// </summary>
        public DateTimeOffset UsageEndTime { get; set; }

        /// <summary>
        /// Gets or sets the start of the usage aggregation time range. The response is grouped
        /// by the time of consumption (when the resource was actually used VS. when was
        /// it reported to the billing system).
        /// </summary>
        public DateTimeOffset UsageStartTime { get; set; }
    }
}