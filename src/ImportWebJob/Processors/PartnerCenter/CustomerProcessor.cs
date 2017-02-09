// -----------------------------------------------------------------------
// <copyright file="CustomerProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors.PartnerCenter
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Enumerations;
    using Storage;
    using Store.PartnerCenter.Models;
    using Store.PartnerCenter.Models.Customers;
    using Store.PartnerCenter.Models.Subscriptions;

    /// <summary>
    /// Processes customers from Partner Center.
    /// </summary>
    public class CustomerProcessor : IProcessor
    {
        /// <summary>
        /// Partition key value for the entities being stored in the customers table.
        /// </summary>
        private const string CustomersPartionKey = "customers";

        /// <summary>
        /// Name of the customers table.
        /// </summary>
        private const string CustomersTableName = "customers";

        /// <summary>
        /// Partition key value for the entities being stored in the subscriptions table.
        /// </summary>
        private const string SubscriptionsPartionKey = "subscriptions";

        /// <summary>
        /// Name of the subscriptions table.
        /// </summary>
        private const string SubscriptionsTableName = "subscriptions";

        /// <summary>
        /// Processes all customers available in Partner Center.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Parallel.ForEach is formatted for readability.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Parallel.ForEach is formatted for readability.")]
        public async Task ProcessAsync()
        {
            Customer customer; 
            CustomerEntity customerEntity;
            List<SubscriptionEntity> entities;
            ResourceCollection<Customer> customers;
            ResourceCollection<Subscription> subscriptions;

            try
            {
                entities = new List<SubscriptionEntity>();
                customers = await Program.Core.PartnerCenter.Customers.GetAsync();

                foreach (Customer c in customers.Items)
                {
                    subscriptions = await Program.Core.PartnerCenter.Customers.ById(c.Id).Subscriptions.GetAsync();
                    entities.AddRange(subscriptions.Items.Select(s => new SubscriptionEntity
                    {
                        BillingType = s.BillingType.ToString(),
                        CustomerId = c.Id,
                        FriendlyName = s.FriendlyName,
                        PartitionKey = SubscriptionsPartionKey,
                        Provider = SubscriptionProvider.PartnerCenter.ToString(),
                        RowKey = s.Id,
                        Status = s.Status.ToString(),
                        SubscriptionId = s.Id
                    }));

                    customer = await Program.Core.PartnerCenter.Customers.ById(c.Id).GetAsync();

                    customerEntity = new CustomerEntity
                    { 
                        City = customer?.BillingProfile?.DefaultAddress?.City,
                        CustomerId = customer.Id,
                        Domain = customer.CompanyProfile.Domain,
                        Name = customer.CompanyProfile.CompanyName,
                        PartitionKey = CustomersPartionKey,
                        PostalCode = customer?.BillingProfile?.DefaultAddress?.PostalCode,
                        RowKey = customer.Id,
                        State = customer.BillingProfile?.DefaultAddress?.State
                    };

                    await Program.Core.Storage.WriteToTableAsync(CustomersTableName, customerEntity);
                }

                Parallel.ForEach(entities, async subscription =>
                {
                    await Program.Core.Storage.WriteToQueueAsync(
                        ApplicationConfiguration.SubscriptionsQueueName, subscription);

                    await Program.Core.Storage.WriteToTableAsync(SubscriptionsTableName, subscription);
                });
            }
            finally
            {
                customer = null; 
                customerEntity = null;
                customers = null;
                subscriptions = null;
            }
        }
    }
}