// -----------------------------------------------------------------------
// <copyright file="CustomerEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Storage
{
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// A customer object store in a storage table.
    /// </summary>
    public class CustomerEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerEntity"/> class.
        /// </summary>
        public CustomerEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">Partition key for the entity.</param>
        /// <param name="rowKey">Row key for the entity.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="partitionKey"/> is null.
        /// or
        /// <paramref name="rowKey"/> is null.
        /// </exception>
        public CustomerEntity(string partitionKey, string rowKey)
        {
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the city where the customer is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the customer.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the domain assigned to the customer.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the zip code where the customer is located.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the state where the customer is located.
        /// </summary>
        public string State { get; set; }
    }
}