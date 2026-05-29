// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Dashboard.Mvc.Infrastructure;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Hubs;
using ChuA.ERP.Web.Mvc.Security;
using Serilog;

#if DEBUG || ENABLE_DEV_BYPASS
using ChuA.Authentication.DevBypass.Extensions;
#endif

namespace ChuA.ERP.Web.Mvc;

/// <summary>Composition root for the ChuA ERP MVC UI host.</summary>
public class Program
{
    public static int Main(string[] args)
    {
        // Bootstrap logger: captures anything that fails before the host (and its
        // configuration-driven sinks) is built. Writes to console and an early file.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/chua-erp-web-bootstrap-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting ChuA ERP MVC host");

            var builder = WebApplication.CreateBuilder(args);

            // Replace the default logging providers with Serilog. Sinks (including the
            // rolling file sink) are read from the "Serilog" section of appsettings.json
            // so operators can change levels/paths without a rebuild.
            builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddChuAErpMvc(builder.Configuration, builder.Environment);

#if DEBUG || ENABLE_DEV_BYPASS
            // DEVELOPMENT-ONLY: replace cookie/OIDC with a synthetic
            // bypass principal so dev sessions skip the Auth0 login
            // redirect entirely. Activated when configuration has
            // Authentication:Bypass=true OR (compat) ChuAAuthentication:Dev:Bypass=true.
            // The library guards itself against Production environments — it
            // returns from registration without touching DI if IsProduction.
            // Configure ERP permission constants on the synthetic principal
            // so [Authorize(Policy = ...)] dashboard endpoints pass without
            // hand-listing every policy name in appsettings.
#pragma warning disable CS0618
            builder.Services.AddChuADevAuthBypass(
                builder.Configuration,
                builder.Environment,
                opts => opts.PermissionConstantSources.Add(typeof(AuthorizationPolicies)));
#pragma warning restore CS0618
#endif

            //Dashboard
            builder.Services.AddControllersWithViews();
            builder.Services.AddChuADashboardMvc(builder.Configuration);  // <-- the module

            builder.Services.AddAuthorization();

            // The module's JS sends antiforgery as a header.
            builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

            var app = builder.Build();

            // Emit one structured log line per HTTP request (method, path, status, elapsed).
            app.UseSerilogRequestLogging();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


            // Make sure your area route is mapped: For Dashboard
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // MVC-hosted SignalR hub for reactive UX (workflow inbox, notification center,
            // per-row partial updates). Cookie auth flows through automatically because the
            // client connection rides the same browser session.
            app.MapHub<ChuaErpHub>(ChuaErpHub.HubPath);

            app.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "ChuA ERP MVC host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
