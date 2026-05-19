using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ChuA.ERP.Web.Mvc.Tests.Configuration;

public class StartupOptionsValidationTests
{
    [Fact]
    public void Production_options_should_reject_auth_bypass()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Api:BaseUrl"] = "https://localhost:5001",
                ["Api:TimeoutSeconds"] = "30",
                ["Api:RetryCount"] = "3",
                ["Api:UseAuthBypass"] = "true",
                ["Oidc:Enabled"] = "true",
                ["Oidc:Authority"] = "https://identity.example.test",
                ["Oidc:ClientId"] = "chua-erp-mvc",
                ["Oidc:ClientSecret"] = "secret"
            })
            .Build();

        services.AddChuAErpMvc(configuration, new TestWebHostEnvironment(Environments.Production));

        using var provider = services.BuildServiceProvider();

        var action = () => provider.GetRequiredService<IOptions<ApiOptions>>().Value;
        action.Should().Throw<OptionsValidationException>()
            .WithMessage("*Api:UseAuthBypass must be false outside Development*");
    }

    [Fact]
    public void Development_options_should_allow_auth_bypass()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Api:BaseUrl"] = "https://localhost:5001",
                ["Api:TimeoutSeconds"] = "30",
                ["Api:RetryCount"] = "3",
                ["Api:UseAuthBypass"] = "true",
                ["Oidc:Enabled"] = "false"
            })
            .Build();

        services.AddChuAErpMvc(configuration, new TestWebHostEnvironment(Environments.Development));

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IOptions<ApiOptions>>().Value.UseAuthBypass.Should().BeTrue();
        provider.GetRequiredService<IOptions<OidcOptions>>().Value.Enabled.Should().BeFalse();
    }

    private sealed class TestWebHostEnvironment(string environmentName) : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "ChuA.ERP.Web.Mvc.Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
