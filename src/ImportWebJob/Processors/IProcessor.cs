// -----------------------------------------------------------------------
// <copyright file="IProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an entity processor.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Performs the necessary processing upon the entity.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task ProcessAsync();
    }

    /// <summary>
    /// Represents an entity processor.
    /// </summary>
    /// <typeparam name="T">The type of entity being processed.</typeparam>
    public interface IProcessor<in T>
    {
        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">An aptly populate entity.</param>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <see cref="entity"/> is null.
        /// </exception>
        Task ProcessAsync(T entity);
    }
}