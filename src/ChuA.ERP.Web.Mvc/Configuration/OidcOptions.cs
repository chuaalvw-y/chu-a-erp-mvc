namespace ChuA.ERP.Web.Mvc.Configuration;

/// <summary>
/// OpenID Connect options. Placeholder configuration; when populated the MVC will
/// wire OIDC sign-in via Microsoft.AspNetCore.Authentication.OpenIdConnect.
/// Bound to the "Oidc" configuration section.
/// </summary>
public sealed class OidcOptions
{
    public const string SectionName = "Oidc";

    /// <summary>Set true to enable OIDC sign-in instead of cookie-only stub auth.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>OIDC authority URL (e.g. https://identity.chua.example.com).</summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>Client identifier registered with the IdP.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>Client secret (use user-secrets or env var; do not commit).</summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Sign-in callback path; defaults to /signin-oidc.</summary>
    public string CallbackPath { get; set; } = "/signin-oidc";

    /// <summary>Sign-out callback path; defaults to /signout-callback-oidc.</summary>
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";

    /// <summary>Scopes to request, e.g. openid profile email chua-erp-api.</summary>
    public List<string> Scopes { get; set; } = new() { "openid", "profile", "email" };

    /// <summary>Audience expected on tokens used against the API.</summary>
    public string ApiAudience { get; set; } = "chua-erp-api";

    /// <summary>True to require HTTPS metadata (false only in dev).</summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
