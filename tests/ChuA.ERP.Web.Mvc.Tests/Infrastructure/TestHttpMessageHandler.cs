using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace ChuA.ERP.Web.Mvc.Tests.Infrastructure;

/// <summary>
/// HttpClient handler used in unit tests. Captures the outbound request and returns a
/// pre-configured response — lets us assert that ApiClient classes attach headers,
/// route URIs correctly, and serialize bodies the way we expect.
/// </summary>
internal sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public List<HttpRequestMessage> Requests { get; } = new();

    /// <summary>Eagerly captured request bodies — Content is disposed by the SUT before the test asserts.</summary>
    public List<string?> RequestBodies { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        RequestBodies.Add(request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken));
        return _responder(request);
    }

    public static TestHttpMessageHandler RespondJson<T>(T body, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new TestHttpMessageHandler(_ => new HttpResponseMessage(status)
        {
            Content = JsonContent.Create(body),
        });
    }

    public static TestHttpMessageHandler RespondNoContent() =>
        new(_ => new HttpResponseMessage(HttpStatusCode.NoContent));

    public static TestHttpMessageHandler RespondProblem(int status, string title, string? errorCode = null, string? detail = null)
    {
        var problem = new
        {
            type = "https://example.com/error",
            title,
            status,
            detail = detail ?? title,
            errorCode,
            correlationId = "test-correlation",
            traceId = "test-trace",
        };
        return new TestHttpMessageHandler(_ => new HttpResponseMessage((HttpStatusCode)status)
        {
            Content = JsonContent.Create(problem, options: null),
        });
    }
}
