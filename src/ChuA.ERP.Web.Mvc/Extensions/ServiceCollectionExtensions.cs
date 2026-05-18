using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Configuration;
using ChuA.ERP.Web.Mvc.Filters;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ChuA.ERP.Web.Mvc.Extensions;

/// <summary>
/// Hosts the composition root for the MVC layer: options binding, authentication, authorization
/// policies, API clients, and infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds every MVC-layer dependency.</summary>
    public static IServiceCollection AddChuAErpMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddChuAOptions(configuration)
            .AddChuACoreServices()
            .AddChuAAuthN(configuration)
            .AddChuAAuthZ()
            .AddChuAApiClients(configuration);

        services.AddControllersWithViews(options =>
        {
            options.Filters.Add<CorrelationIdActionFilter>();
            options.Filters.Add<GlobalExceptionFilter>();
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        services.AddHttpContextAccessor();
        services.AddRazorPages();
        return services;
    }

    private static IServiceCollection AddChuAOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        services.Configure<OidcOptions>(configuration.GetSection(OidcOptions.SectionName));
        services.Configure<AppEnvironmentOptions>(configuration.GetSection(AppEnvironmentOptions.SectionName));
        return services;
    }

    private static IServiceCollection AddChuACoreServices(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddScoped<ITokenAcquisitionService, CookieTokenAcquisitionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<CorrelationIdActionFilter>();
        services.AddScoped<GlobalExceptionFilter>();
        return services;
    }

    private static IServiceCollection AddChuAAuthN(this IServiceCollection services, IConfiguration configuration)
    {
        var oidc = configuration.GetSection(OidcOptions.SectionName).Get<OidcOptions>() ?? new OidcOptions();

        var auth = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = oidc.Enabled
                ? OpenIdConnectDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;
        });

        auth.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.Name = "ChuA.ERP.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        if (oidc.Enabled)
        {
            auth.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = oidc.Authority;
                options.ClientId = oidc.ClientId;
                options.ClientSecret = oidc.ClientSecret;
                options.CallbackPath = oidc.CallbackPath;
                options.SignedOutCallbackPath = oidc.SignedOutCallbackPath;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = oidc.RequireHttpsMetadata;
                options.Scope.Clear();
                foreach (var scope in oidc.Scopes)
                {
                    options.Scope.Add(scope);
                }
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "role";
            });
        }

        return services;
    }

    private static IServiceCollection AddChuAAuthZ(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var policyName in AuthorizationPolicies.All)
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    if (policyName == AuthorizationPolicies.AuthenticatedUser) return;

                    if (policyName == AuthorizationPolicies.SystemAdmin)
                    {
                        policy.RequireRole("SystemAdmin");
                        return;
                    }
                    if (policyName == AuthorizationPolicies.CompanyAdmin)
                    {
                        policy.RequireRole("SystemAdmin", "CompanyAdmin");
                        return;
                    }

                    policy.RequireAssertion(ctx =>
                        ctx.User.HasClaim(c => c.Type == "permission" && c.Value == policyName)
                        || ctx.User.IsInRole("SystemAdmin"));
                });
            }
        });
        return services;
    }

    private static IServiceCollection AddChuAApiClients(this IServiceCollection services, IConfiguration configuration)
    {
        var api = configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>() ?? new ApiOptions();
        var baseAddress = new Uri(api.BaseUrl.TrimEnd('/') + "/api/");

        void Configure(HttpClient client)
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(api.TimeoutSeconds);
        }

        services.AddHttpClient<IHealthApiClient, HealthApiClient>(Configure);
        services.AddHttpClient<IUsersApiClient, UsersApiClient>(Configure);
        services.AddHttpClient<IVendorsApiClient, VendorsApiClient>(Configure);
        services.AddHttpClient<ICustomersApiClient, CustomersApiClient>(Configure);
        services.AddHttpClient<IChartOfAccountsApiClient, ChartOfAccountsApiClient>(Configure);
        services.AddHttpClient<IJournalEntriesApiClient, JournalEntriesApiClient>(Configure);
        services.AddHttpClient<IBillsApiClient, BillsApiClient>(Configure);
        services.AddHttpClient<IInvoicesApiClient, InvoicesApiClient>(Configure);
        services.AddHttpClient<IPurchaseOrdersApiClient, PurchaseOrdersApiClient>(Configure);
        services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(Configure);
        services.AddHttpClient<ISalesOrdersApiClient, SalesOrdersApiClient>(Configure);
        services.AddHttpClient<IWorkflowApiClient, WorkflowApiClient>(Configure);
        services.AddHttpClient<ICompaniesApiClient, CompaniesApiClient>(Configure);
        services.AddHttpClient<IRolesApiClient, RolesApiClient>(Configure);
        services.AddHttpClient<IReportsApiClient, ReportsApiClient>(Configure);

        return services;
    }
}
