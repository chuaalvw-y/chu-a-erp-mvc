using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Configuration;
using ChuA.ERP.Web.Mvc.Filters;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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

        services.AddOptions<OidcOptions>()
            .Bind(configuration.GetSection(OidcOptions.SectionName))
            .Validate(options => environment.IsDevelopment() || options.Enabled, "Oidc:Enabled must be true outside Development.")
            .Validate(options => environment.IsDevelopment() || !string.IsNullOrWhiteSpace(options.Authority), "Oidc:Authority is required outside Development.")
            .Validate(options => environment.IsDevelopment() || !string.IsNullOrWhiteSpace(options.ClientId), "Oidc:ClientId is required outside Development.")
            .Validate(options => environment.IsDevelopment() || !string.IsNullOrWhiteSpace(options.ClientSecret), "Oidc:ClientSecret is required outside Development.")
            .ValidateOnStart();

        services.AddOptions<AppEnvironmentOptions>()
            .Bind(configuration.GetSection(AppEnvironmentOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddChuACoreServices(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddScoped<ITokenAcquisitionService, CookieTokenAcquisitionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<CorrelationIdActionFilter>();
        services.AddScoped<GlobalExceptionFilter>();
        services.AddMemoryCache();
        services.AddSingleton<ITicketStore, MemoryCacheTicketStore>();
        return services;
    }

    private static IServiceCollection AddChuAAuthN(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
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
