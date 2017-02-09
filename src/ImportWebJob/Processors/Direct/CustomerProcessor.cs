// -----------------------------------------------------------------------
// <copyright file="CustomerProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors.Direct
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Processes customers direct from Microsoft.
    /// </summary>
    public class CustomerProcessor : IProcessor
    {
        /// <summary>
        /// Performs the necessary processing upon the entity.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }
}