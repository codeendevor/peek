// -----------------------------------------------------------------------
// <copyright file="TokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Authentication
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using IdentityModel.Clients.ActiveDirectory;
    using Store.PartnerCenter;
    using Store.PartnerCenter.Extensions;

    /// <summary>
    /// Provides access token management functionality.
    /// </summary>
    public class TokenManagement : ITokenManagement
    {
        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="applicationId">Identifier of the application requesting the token.</param>
        /// <param name="applicationSecret">Secret of the application requesting the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationResult"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is null.
        /// or
        /// <paramref name="applicationId"/> is null.
        /// or
        /// <paramref name="applicationSecret"/> is null.
        /// or
        /// <paramref name="resource"/> is null.
        /// </exception>
        public AuthenticationResult GetAppOnlyToken(string authority, string applicationId, string applicationSecret, string resource)
        {
            authority.AssertNotEmpty(nameof(authority));
            applicationId.AssertNotEmpty(nameof(applicationId));
            applicationSecret.AssertNotEmpty(nameof(applicationSecret));
            resource.AssertNotEmpty(nameof(resource));

            return SynchronousExecute(() => this.GetAppOnlyTokenAsync(authority, applicationId, applicationSecret, resource));
        }

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="applicationId">Identifier of the application requesting the token.</param>
        /// <param name="applicationSecret">Secret of the application requesting the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationResult"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is null.
        /// or
        /// <paramref name="applicationId"/> is null.
        /// or
        /// <paramref name="applicationSecret"/> is null.
        /// or
        /// <paramref name="resource"/> is null.
        /// </exception>
        public async Task<AuthenticationResult> GetAppOnlyTokenAsync(string authority, string applicationId, string applicationSecret, string resource)
        {
            AuthenticationContext authContext;

            authority.AssertNotEmpty(nameof(authority));
            applicationId.AssertNotEmpty(nameof(applicationId));
            applicationSecret.AssertNotEmpty(nameof(applicationSecret));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                authContext = new AuthenticationContext(authority);

                return await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        applicationId,
                        applicationSecret));
            }
            finally
            {
                authContext = null;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            return SynchronousExecute(() => this.GetPartnerCenterAppOnlyCredentialsAsync(authority));
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public async Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            IPartnerCredentials credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                ApplicationConfiguration.PartnerCenterApplicationId,
                ApplicationConfiguration.PartnerCenterApplicationSecret,
                ApplicationConfiguration.PartnerCenterTenantId);

            return credentials;
        }

        /// <summary>
        /// Executes the asynchronous function synchronously.
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="operation">The asynchronous function to be invoked.</param>
        /// <returns>The results from the asynchronous operation.</returns>
        private static T SynchronousExecute<T>(Func<Task<T>> operation)
        {
            try
            {
                return Task.Run(async () => await operation()).Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}