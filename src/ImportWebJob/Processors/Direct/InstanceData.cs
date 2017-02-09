// -----------------------------------------------------------------------
// <copyright file="InstanceData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Processors.Direct
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents instance data for an Azure usage record.
    /// </summary>
    public class InstanceData
    {
        /// <summary>
        /// Gets or sets the region in which the service was run. 
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified resource ID, which includes the resource groups and the instance name.
        /// </summary>
        [JsonProperty("resourceURI")]
        public Uri ResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the resource tags specified by the user.
        /// </summary>
        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="InstanceData"/> from the specified JSON.
        /// </summary>
        /// <param name="instanceData">JSON representation of the <see cref="InstanceData"/> class.</param>
        /// <returns>Instance data for the resource.</returns>
        public static InstanceData GetInstance(string instanceData)
        {
            instanceData.AssertNotEmpty(nameof(instanceData));

            return JsonConvert.DeserializeObject<InstanceData>(instanceData);
        }
    }
}