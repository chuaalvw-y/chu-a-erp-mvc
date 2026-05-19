using ChuA.ERP.Web.Mvc.Extensions;

namespace ChuA.ERP.Web.Mvc;

/// <summary>Composition root for the ChuA ERP MVC UI host.</summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddChuAErpMvc(builder.Configuration, builder.Environment);

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

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        app.Run();
    }
}
