using System.Net;
using ChuA.ERP.Web.Mvc.Tests.Infrastructure;

namespace ChuA.ERP.Web.Mvc.Tests.ApiClients;

public class VendorsApiClientTests
{
    [Fact]
    public async Task ListAsync_should_call_v1_vendors_with_search_query()
    {
        var dto = new VendorDto(Guid.NewGuid(), Guid.NewGuid(), "V100", "Acme", "USD", 30, false);
        var handler = TestHttpMessageHandler.RespondJson<IReadOnlyList<VendorDto>>(new[] { dto });
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);

        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        var result = await sut.ListAsync("acme");

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(v => v.VendorCode == "V100");
        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Method.Should().Be(HttpMethod.Get);
        handler.Requests[0].RequestUri!.PathAndQuery.Should().Be("/api/v1/vendors?search=acme&pageNumber=1&pageSize=25");
    }

    [Fact]
    public async Task ListAsync_should_attach_correlation_header()
    {
        var handler = TestHttpMessageHandler.RespondJson<IReadOnlyList<VendorDto>>(Array.Empty<VendorDto>());
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        correlation.Setup(c => c.GetOrCreate()).Returns("correlation-abc");

        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        await sut.ListAsync();

        handler.Requests[0].Headers.GetValues(ApiHeaders.CorrelationId).Should().ContainSingle().Which.Should().Be("correlation-abc");
    }

    [Fact]
    public async Task CreateAsync_should_POST_request_body_to_v1_vendors()
    {
        var dto = new VendorDto(Guid.NewGuid(), Guid.NewGuid(), "V200", "Beta Co", "EUR", 45, false);
        var handler = TestHttpMessageHandler.RespondJson(dto, HttpStatusCode.Created);
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        var request = new CreateVendorRequest("V200", "Beta Co", "EUR", 45);
        var result = await sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.VendorCode.Should().Be("V200");
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be("/api/v1/vendors");
        handler.RequestBodies[0].Should().Contain("\"vendorCode\":\"V200\"");
    }

    [Fact]
    public async Task DeleteAsync_should_treat_204_as_success()
    {
        var handler = TestHttpMessageHandler.RespondNoContent();
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        var result = await sut.DeleteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        handler.Requests[0].Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task ProblemDetails_response_should_become_failed_Result()
    {
        var handler = TestHttpMessageHandler.RespondProblem(409, "Conflict", "vendor.code.already", "Vendor code already exists");
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        var result = await sut.CreateAsync(new CreateVendorRequest("V300", "Gamma", "USD", 30));

        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(409);
        result.Problem!.ErrorCode.Should().Be("vendor.code.already");
    }

    [Fact]
    public async Task BearerToken_should_be_attached_when_acquired()
    {
        var handler = TestHttpMessageHandler.RespondJson<IReadOnlyList<VendorDto>>(Array.Empty<VendorDto>());
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        token.Setup(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("test-token-abc");
        var sut = new VendorsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<VendorsApiClient>());

        await sut.ListAsync();

        handler.Requests[0].Headers.Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests[0].Headers.Authorization!.Parameter.Should().Be("test-token-abc");
    }
}
