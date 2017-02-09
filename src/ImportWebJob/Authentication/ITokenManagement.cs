// -----------------------------------------------------------------------
// <copyright file="ITokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Peek.ImportWebJob.Authentication
{
    using System.Threading.Tasks;
    using IdentityModel.Clients.ActiveDirectory;
    using Store.PartnerCenter;

    /// <summary>
    /// Provides a management interface for managing access tokens.
    /// </summary>
    public interface ITokenManagement
    {
        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="applicationId">Identifier of the application requesting the token.</param>
        /// <param name="applicationSecret">Secret of the application requesting the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationResult"/> that represented the access token.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="authority"/> is null.
        /// or
        /// <paramref name="applicationId"/> is null.
        /// or
        /// <paramref name="applicationSecret"/> is null.
        /// or
        /// <paramref name="resource"/> is null.
        /// </exception>
        AuthenticationResult GetAppOnlyToken(string authority, string applicationId, string applicationSecret, string resource);

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="applicationId">Identifier of the application requesting the token.</param>
        /// <param name="applicationSecret">Secret of the application requesting the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationResult"/> that represented the access token.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="authority"/> is null.
        /// or
        /// <paramref name="applicationId"/> is null.
        /// or
        /// <paramref name="applicationSecret"/> is null.
        /// or
        /// <paramref name="resource"/> is null.
        /// </exception>
        Task<AuthenticationResult> GetAppOnlyTokenAsync(string authority, string applicationId, string applicationSecret, string resource);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority);
    }
}