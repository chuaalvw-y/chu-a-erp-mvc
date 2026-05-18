using ChuA.ERP.Web.Mvc.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Default token acquisition: looks for an <c>access_token</c> stored in the cookie auth
/// session (populated when OIDC is wired up). Falls back to a configured development bearer
/// token, or to no token at all when the API is using AuthBypass.
/// </summary>
public sealed class CookieTokenAcquisitionService : ITokenAcquisitionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<ApiOptions> _apiOptions;

    public CookieTokenAcquisitionService(
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<ApiOptions> apiOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiOptions = apiOptions;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var options = _apiOptions.CurrentValue;
        if (options.UseAuthBypass)
        {
            return null;
        }

        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is not null)
        {
            var token = await ctx.GetTokenAsync("access_token").ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
        }

        return string.IsNullOrWhiteSpace(options.DevelopmentBearerToken) ? null : options.DevelopmentBearerToken;
    }
}
