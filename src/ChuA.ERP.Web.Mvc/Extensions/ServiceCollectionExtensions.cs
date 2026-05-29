// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.Authentication.Constants;
using ChuA.Authentication.Extensions;
#if DEBUG || ENABLE_DEV_BYPASS
using ChuA.Authentication.DevBypass.Extensions;
#endif
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Configuration;
using ChuA.ERP.Web.Mvc.Filters;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace ChuA.ERP.Web.Mvc.Extensions;

/// <summary>
/// Hosts the composition root for the MVC layer: options binding, authentication, authorization
/// policies, API clients, and infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds every MVC-layer dependency.</summary>
    public static IServiceCollection AddChuAErpMvc(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment? environment = null)
    {
        environment ??= new ProductionEnvironment();

        services
            .AddChuAOptions(configuration, environment)
            .AddChuACoreServices()
            .AddChuAAuthN(configuration, environment)
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

    private static IServiceCollection AddChuAOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddOptions<ApiOptions>()
            .Bind(configuration.GetSection(ApiOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "Api:BaseUrl must be an absolute URL.")
            .Validate(options => options.TimeoutSeconds > 0, "Api:TimeoutSeconds must be greater than zero.")
            .Validate(options => options.RetryCount >= 0, "Api:RetryCount cannot be negative.")
            .Validate(options => environment.IsDevelopment() || !options.UseAuthBypass, "Api:UseAuthBypass must be false outside Development.")
            .ValidateOnStart();

        // OidcOptions is now legacy. Auth wiring lives under the "ChuAAuthentication" section
        // and flows through ChuA.Authentication. The binding is kept so older callers that still
        // resolve IOptions<OidcOptions> (and the StartupOptionsValidationTests) keep working.
        services.AddOptions<OidcOptions>()
            .Bind(configuration.GetSection(OidcOptions.SectionName));

        services.AddOptions<AppEnvironmentOptions>()
            .Bind(configuration.GetSection(AppEnvironmentOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddChuACoreServices(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddScoped<ITokenAcquisitionService, CookieTokenAcquisitionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        // Phase J — hydrates role/permission/erp_user_id claims from the ERP database
        // (via /api/v1/users/me) onto every authenticated principal. Plugs into the
        // library's single IClaimsTransformation via the IClaimsEnricher contract.
        // The library handles MapClaims + per-request re-entry guard; we just stamp
        // ERP claims and let the library do the rest.
        services.AddScoped<ChuA.Authentication.Claims.IClaimsEnricher,
                           Security.ErpClaimsTransformation>();
        services.AddScoped<CorrelationIdActionFilter>();
        services.AddScoped<GlobalExceptionFilter>();
        services.AddMemoryCache();
        services.AddSingleton<ITicketStore, MemoryCacheTicketStore>();
        // Pure function over a ClaimsPrincipal; singleton is safe and avoids per-request allocations.
        services.AddSingleton<IAuthDebugSnapshotBuilder, AuthDebugSnapshotBuilder>();

        // Reactive UX infrastructure (Wave 1): in-memory notification store + SignalR-backed publisher.
        // Store is a singleton — per-user notifications survive across requests within the same MVC process.
        // Publisher is also a singleton; its only dependencies (IHubContext, store, logger) are themselves singletons.
        services.AddSingleton<INotificationStore, InMemoryNotificationStore>();
        services.AddSingleton<INotificationPublisher, HubNotificationPublisher>();

        // Host-side SignalR. The MVC owns ChuaErpHub; the client connects to /hubs/erp from chua-reactive.js.
        services.AddSignalR();

        return services;
    }

    private static IServiceCollection AddChuAAuthN(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Auth wiring is delegated to the ChuA.Authentication shared library. We merge a default
        // ChuAAuthentication shape underneath the host configuration so callers that have not yet
        // populated the section still get a coherent cookie + OIDC web-app wiring (cookie-only
        // when ClientId is empty — preserves the DevLogin path in Development).
        var configurationWithAuthDefaults = new ConfigurationBuilder()
            .AddInMemoryCollection(BuildChuAAuthenticationDefaults())
            .AddConfiguration(configuration)
            .Build();

        services.AddChuAAuthentication(
            configurationWithAuthDefaults,
            ChuAAuthenticationDefaults.ConfigurationSectionName);

#if DEBUG || ENABLE_DEV_BYPASS
        // DEVELOPMENT-ONLY: replace the cookie/OIDC default scheme with a
        // synthetic bypass principal so dev sessions skip the Auth0 login
        // redirect entirely. Activated when configuration has
        // Authentication:Bypass=true OR (compat) ChuAAuthentication:Dev:Bypass=true.
        // The library self-guards against IHostEnvironment.IsProduction —
        // calling AddChuADevAuthBypass in Production is a no-op.
        // Configure ERP permission constants on the synthetic principal so
        // [Authorize(Policy = ...)] dashboard endpoints pass without
        // hand-listing every policy name in appsettings.
#pragma warning disable CS0618
        services.AddChuADevAuthBypass(
            configuration,
            environment,
            opts => opts.PermissionConstantSources.Add(typeof(AuthorizationPolicies)));
#pragma warning restore CS0618
#endif

        // PostConfigure the cookie scheme that ChuA.Authentication registered with the
        // ERP-specific paths, cookie name, expiry, and (in Production) a server-side ticket store.
        services.PostConfigure<CookieAuthenticationOptions>(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "ChuA.ERP.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .Configure<ITicketStore>((options, ticketStore) =>
            {
                if (!environment.IsDevelopment())
                {
                    options.SessionStore = ticketStore;
                }
            });

        return services;
    }

    /// <summary>
    /// Default ChuAAuthentication shape: an Auth0-backed cookie + OIDC code-flow web app with
    /// empty credentials. The OidcWebApp configurator falls back to cookie-only registration
    /// when ClientId is empty, so the DevLogin path still works in Development without an IdP.
    /// </summary>
    private static Dictionary<string, string?> BuildChuAAuthenticationDefaults() => new()
    {
        ["ChuAAuthentication:DefaultProvider"] = "Auth0",
        ["ChuAAuthentication:RequireAuthenticatedUserByDefault"] = "false",
        ["ChuAAuthentication:EnableClaimsTransformation"] = "true",
        ["ChuAAuthentication:NameClaimType"] = "preferred_username",
        ["ChuAAuthentication:RoleClaimType"] = "role",
        ["ChuAAuthentication:Providers:Auth0:Type"] = "Auth0WebApp",
        ["ChuAAuthentication:Providers:Auth0:Scheme"] = "OpenIdConnect",
        ["ChuAAuthentication:Providers:Auth0:CookieScheme"] = "Cookies",
        ["ChuAAuthentication:Providers:Auth0:CallbackPath"] = "/signin-oidc",
        ["ChuAAuthentication:Providers:Auth0:SignedOutCallbackPath"] = "/signout-callback-oidc",
        ["ChuAAuthentication:Providers:Auth0:Scopes:0"] = "openid",
        ["ChuAAuthentication:Providers:Auth0:Scopes:1"] = "profile",
        ["ChuAAuthentication:Providers:Auth0:Scopes:2"] = "email",
        ["ChuAAuthentication:Providers:Auth0:RequireHttpsMetadata"] = "true",
        ["ChuAAuthentication:Providers:Auth0:UsePkce"] = "true",
        ["ChuAAuthentication:Providers:Auth0:SaveTokens"] = "true",
        ["ChuAAuthentication:Providers:Auth0:GetClaimsFromUserInfoEndpoint"] = "true",
    };

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
        ValidateApiOptions(api);
        var baseAddress = new Uri(api.BaseUrl.TrimEnd('/') + "/api/");

        void Configure(HttpClient client)
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(api.TimeoutSeconds);
        }

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(api.RetryCount, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)));
        var noRetryPolicy = Policy.NoOpAsync<HttpResponseMessage>();

        IHttpClientBuilder AddApiClient<TClient, TImplementation>()
            where TClient : class
            where TImplementation : class, TClient =>
            services.AddHttpClient<TClient, TImplementation>(Configure)
                .AddPolicyHandler(request => IsSafeToRetry(request.Method) ? retryPolicy : noRetryPolicy);

        AddApiClient<IHealthApiClient, HealthApiClient>();
        AddApiClient<IUsersApiClient, UsersApiClient>();
        AddApiClient<IVendorsApiClient, VendorsApiClient>();
        AddApiClient<ICustomersApiClient, CustomersApiClient>();
        AddApiClient<IChartOfAccountsApiClient, ChartOfAccountsApiClient>();
        AddApiClient<IJournalEntriesApiClient, JournalEntriesApiClient>();
        AddApiClient<IBillsApiClient, BillsApiClient>();
        AddApiClient<IInvoicesApiClient, InvoicesApiClient>();
        AddApiClient<IPurchaseOrdersApiClient, PurchaseOrdersApiClient>();
        AddApiClient<IInventoryApiClient, InventoryApiClient>();
        AddApiClient<ISalesOrdersApiClient, SalesOrdersApiClient>();
        AddApiClient<IWorkflowApiClient, WorkflowApiClient>();
        AddApiClient<IWorkflowDefinitionsApiClient, WorkflowDefinitionsApiClient>();
        AddApiClient<IWorkflowConfigurationsApiClient, WorkflowConfigurationsApiClient>();
        AddApiClient<IWorkflowInstancesApiClient, WorkflowInstancesApiClient>();
        AddApiClient<IBusinessRulesApiClient, BusinessRulesApiClient>();
        AddApiClient<ICompaniesApiClient, CompaniesApiClient>();
        AddApiClient<IRolesApiClient, RolesApiClient>();
        AddApiClient<IReportsApiClient, ReportsApiClient>();

        return services;
    }

    private static bool IsSafeToRetry(HttpMethod method) =>
        method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Options;

    private static void ValidateApiOptions(ApiOptions api)
    {
        var failures = new List<string>();
        if (!Uri.TryCreate(api.BaseUrl, UriKind.Absolute, out _))
        {
            failures.Add("Api:BaseUrl must be an absolute URL.");
        }
        if (api.TimeoutSeconds <= 0)
        {
            failures.Add("Api:TimeoutSeconds must be greater than zero.");
        }
        if (api.RetryCount < 0)
        {
            failures.Add("Api:RetryCount cannot be negative.");
        }
        if (failures.Count > 0)
        {
            throw new OptionsValidationException(ApiOptions.SectionName, typeof(ApiOptions), failures);
        }
    }

    private sealed class ProductionEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "ChuA.ERP.Web.Mvc";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
