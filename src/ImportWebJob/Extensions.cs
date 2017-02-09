// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob
{
    using System;

    /// <summary>
    /// Provides useful methods used for validation.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Ensures that a string is not empty.
        /// </summary>
        /// <param name="nonEmptyString">The string to validate.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="nonEmptyString"/> is either empty or null.
        /// </exception>
        public static void AssertNotEmpty(this string nonEmptyString, string caption)
        {
            if (string.IsNullOrWhiteSpace(nonEmptyString))
            {
                throw new ArgumentException($"{caption ?? "string"} is not set");
            }
        }

        /// <summary>
        /// Ensures that a given object is not null. Throws an exception otherwise.
        /// </summary>
        /// <param name="objectToValidate">The object we are validating.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="objectToValidate"/> is null.
        /// </exception>
        public static void AssertNotNull(this object objectToValidate, string caption)
        {
            if (objectToValidate == null)
            {
                throw new ArgumentNullException(caption);
            }
        }
    }
}