// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Dashboard.Mvc.Infrastructure;
using ChuA.ERP.Web.Mvc.Extensions;

namespace ChuA.ERP.Web.Mvc;

/// <summary>Composition root for the ChuA ERP MVC UI host.</summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddChuAErpMvc(builder.Configuration, builder.Environment);

        //Dashboard
        builder.Services.AddControllersWithViews();
        builder.Services.AddChuADashboardMvc(builder.Configuration);  // <-- the module

        builder.Services.AddAuthorization();

        // The module's JS sends antiforgery as a header.
        builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

        var app = builder.Build();

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

        app.Run();
    }
}
