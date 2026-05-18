namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Acquires a bearer token to attach to outbound API calls. Implementations may pull tokens
/// from the OIDC session, exchange refresh tokens, or return a fixed development token.
/// </summary>
public interface ITokenAcquisitionService
{
    /// <summary>Returns a bearer access token, or null if the call should be unauthenticated.</summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
