using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ChuA.ERP.Web.Mvc.Tests.Infrastructure;

/// <summary>Factory helpers that produce ready-to-use ApiClient instances over a TestHttpMessageHandler.</summary>
internal static class TestServices
{
    public static (HttpClient Client, Mock<ITokenAcquisitionService> Token, Mock<ICorrelationIdAccessor> Correlation)
        BuildHttpStack(TestHttpMessageHandler handler, Uri? baseAddress = null)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = baseAddress ?? new Uri("https://api.local/api/"),
        };
        var token = new Mock<ITokenAcquisitionService>();
        token.Setup(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);
        var correlation = new Mock<ICorrelationIdAccessor>();
        correlation.Setup(c => c.GetOrCreate()).Returns("test-correlation");
        return (client, token, correlation);
    }

    public static NullLogger<T> NullLog<T>() => NullLogger<T>.Instance;
}
