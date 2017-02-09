// -----------------------------------------------------------------------
// <copyright file="StorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Services
{
    using System;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Newtonsoft.Json;
    using Storage;
    using WindowsAzure.Storage;
    using WindowsAzure.Storage.Blob;
    using WindowsAzure.Storage.Queue;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Provides the ability to interact with an Azure storage account.
    /// </summary>
    public class StorageService : IStorageService
    {
        /// <summary>
        /// An instance of the <see cref="CloudStorageAccount"/> class.
        /// </summary>
        private static CloudStorageAccount storageAccount;

        /// <summary>
        /// An instance of the <see cref="CloudBlobClient"/> class.
        /// </summary>
        private static CloudBlobClient blobClient;

        /// <summary>
        /// An instance of the <see cref="CloudQueueClient"/> class.
        /// </summary>
        private static CloudQueueClient queueClient;

        /// <summary>
        /// An instance of the <see cref="CloudTableClient"/> class.
        /// </summary>
        private static CloudTableClient tableClient;

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudStorageAccount"/> class.
        /// </summary>
        private static CloudStorageAccount StorageAccount => storageAccount ??
            (storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString));

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudBlobClient"/> class.
        /// </summary>
        private static CloudBlobClient BlobClient => blobClient ?? (blobClient = StorageAccount.CreateCloudBlobClient());

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudQueueClient"/> class.
        /// </summary>
        private static CloudQueueClient QueueClient => queueClient ?? (queueClient = StorageAccount.CreateCloudQueueClient());

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudTableClient"/> class.
        /// </summary>
        private static CloudTableClient TableClient => tableClient ?? (tableClient = StorageAccount.CreateCloudTableClient());

        /// <summary>
        /// Resets the Azure storage account if development mode is enabled.
        /// </summary>
        /// <remarks>
        /// This function will remove all components from the storage account. It should NOT be used in 
        /// any production deployment because these operations are irreversible.  
        /// </remarks>
        public void DevelopmentModeCleanup()
        {
            CloudBlobContainer container;
            CloudQueue queue;
            CloudTable table;

            try
            {
                if (!ApplicationConfiguration.IsDevelopment)
                {
                    return;
                }

                container = BlobClient.GetContainerReference("usage");
                container.DeleteIfExists();

                container = BlobClient.GetContainerReference("license");
                container.DeleteIfExists();

                queue = QueueClient.GetQueueReference("subscriptions");
                queue.DeleteIfExists();

                queue = QueueClient.GetQueueReference("subscriptions-poison");
                queue.DeleteIfExists();

                table = TableClient.GetTableReference("azureusage");
                table.DeleteIfExists();

                table = TableClient.GetTableReference("customers");
                table.DeleteIfExists();

                table = TableClient.GetTableReference("licenseskus");
                table.DeleteIfExists();

                table = TableClient.GetTableReference("subscriptions");
                table.DeleteIfExists();

                Thread.Sleep(60000);
            }
            finally
            {
                container = null;
                queue = null;
                table = null;
            }
        }

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
        /// <exception cref="ArgumentException">
        /// containerName
        /// or
        /// blobName
        /// </exception>    
        /// <exception cref="ArgumentNullException">
        /// item
        /// </exception>
        public async Task WriteToBlobAsync<T>(string containerName, string blobName, T item)
        {
            CloudBlockBlob blob;
            CloudBlobContainer container;

            containerName.AssertNotEmpty(nameof(containerName));
            blobName.AssertNotEmpty(nameof(blobName));
            item.AssertNotNull(nameof(item));

            try
            {
                container = BlobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();

                blob = container.GetBlockBlobReference(blobName);
                await blob.UploadTextAsync(JsonConvert.SerializeObject(item));
            }
            finally
            {
                blob = null;
                container = null;
            }
        }

        /// <summary>
        /// Writes them item to the the specified queue. 
        /// </summary>
        /// <typeparam name="T">The type of item to be written to the storage queue.</typeparam>
        /// <param name="queueName">Name of the queue where the item will be written.</param>
        /// <param name="item">The item to be written to the queue.</param>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="queueName"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        public async Task WriteToQueueAsync<T>(string queueName, T item)
        {
            CloudQueue queue;
            CloudQueueMessage message;

            queueName.AssertNotEmpty(nameof(queueName));
            item.AssertNotNull(nameof(item));

            try
            {
                queue = QueueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();

                message = new CloudQueueMessage(JsonConvert.SerializeObject(item));
                await queue.AddMessageAsync(message);
            }
            finally
            {
                message = null;
                queue = null;
            }
        }

        /// <summary>
        /// Writes the entity to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entity">Entity to be written to the table.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is either empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="StorageOperationException">
        /// The PartitionKey and RowKey must be specified.
        /// </exception>
        /// <remarks>
        /// If there is an entity with a matching PartitionKey and RowKey then the entity 
        /// will be replaced. Otherwise, the entity is inserted into the table as new entity.
        /// </remarks>
        public async Task WriteToTableAsync<T>(string tableName, T entity) where T : TableEntity
        {
            CloudTable table;
            TableOperation operation;

            tableName.AssertNotEmpty(nameof(tableName));
            entity.AssertNotNull(nameof(entity));

            if (string.IsNullOrEmpty(entity.PartitionKey))
            {
                throw new StorageOperationException(Resources.NoPartitionKeyException);
            }

            if (string.IsNullOrEmpty(entity.RowKey))
            {
                throw new StorageOperationException(Resources.NoRowKeyException);
            }

            try
            {
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                operation = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(operation);
            }
            catch (Exception ex)
            {
                Program.Core.Telemetry.TrackException(ex);
            }
            finally
            {
                operation = null;
                table = null;
            }
        }
    }
}