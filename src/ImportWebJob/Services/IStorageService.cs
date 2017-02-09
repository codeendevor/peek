// -----------------------------------------------------------------------
// <copyright file="IStorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Services
{
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a way to interact with storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Resets the Azure storage account if development mode is enabled.
        /// </summary>
        /// <remarks>
        /// This function will remove all components from the storage account. It should NOT be used in 
        /// any production deployment because these operations are irreversible.  
        /// </remarks>
        void DevelopmentModeCleanup();

        /// <summary>
        /// Writes them item to the the specified queue. 
        /// </summary>
        /// <typeparam name="T">The type of item to be written to the storage queue.</typeparam>
        /// <param name="queueName">Name of the queue where the item will be written.</param>
        /// <param name="item">The item to be written to the queue.</param>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="queueName"/> is empty or null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        Task WriteToQueueAsync<T>(string queueName, T item);

        /// <summary>
        /// Writes a serialized version of the item to the specified blob.
        /// </summary>
        /// <typeparam name="T">The type of item to be written to the storage blob.</typeparam>
        /// <param name="containerName">Name of the container where the blob should be written.</param>
        /// <param name="blobName">Name of the blob to be written.</param>
        /// <param name="item">The item to be written to the specified blob.</param>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="containerName"/> is empty or null.
        /// or
        /// <paramref name="blobName"/> is empty or null.
        /// </exception>    
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        Task WriteToBlobAsync<T>(string containerName, string blobName, T item);

        /// <summary>
        /// Writes the entity to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entity">Entity to be written to the table.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="Storage.StorageOperationException">
        /// The PartitionKey property is empty or null.
        /// or
        /// The RowKey property is empty or null.
        /// </exception>
        /// <remarks>
        /// If there is an entity with a matching PartitionKey and RowKey then the entity 
        /// will be replaced. Otherwise, the entity is inserted into the table as new entity.
        /// </remarks>
        Task WriteToTableAsync<T>(string tableName, T entity) where T : TableEntity;
    }
}