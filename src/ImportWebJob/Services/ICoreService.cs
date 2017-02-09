// -----------------------------------------------------------------------
// <copyright file="ICoreService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Services
{
    using Store.PartnerCenter;
    using Telemetry;

    /// <summary>
    /// Represents a way to provide quick access to core services.
    /// </summary>
    public interface ICoreService
    {
        /// <summary>
        /// Gets a reference to the Partner Center service.
        /// </summary>
        IAggregatePartner PartnerCenter { get; }

        /// <summary>
        /// Gets a reference to the storage service.
        /// </summary>
        IStorageService Storage { get; }

        /// <summary>
        /// Gets a reference to the the telemetry provider.
        /// </summary>
        /// <remarks>
        /// This is utilized to capture diagnostic information for the application.
        /// </remarks>
        ITelemetryProvider Telemetry { get; }
    }
}