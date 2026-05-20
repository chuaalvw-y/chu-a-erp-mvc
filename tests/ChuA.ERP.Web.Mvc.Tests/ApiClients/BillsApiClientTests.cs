// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Net;
using ChuA.ERP.Web.Mvc.Tests.Infrastructure;

namespace ChuA.ERP.Web.Mvc.Tests.ApiClients;

public class BillsApiClientTests
{
    private static BillDto SampleBill() => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        "B-001",
        new DateOnly(2026, 1, 1),
        new DateOnly(2026, 1, 31),
        "USD", "Open", "Outstanding",
        new MoneyDto(100m, "USD"),
        new MoneyDto(100m, "USD"));

    [Fact]
    public async Task ListAsync_should_include_all_filters_in_query()
    {
        var handler = TestHttpMessageHandler.RespondJson<IReadOnlyList<BillDto>>(new[] { SampleBill() });
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new BillsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<BillsApiClient>());

        var vendorId = Guid.NewGuid();
        await sut.ListAsync(vendorId, "Approved", "Paid", "B-001");

        var qs = handler.Requests[0].RequestUri!.Query;
        qs.Should().Contain($"vendorId={vendorId}");
        qs.Should().Contain("status=Approved");
        qs.Should().Contain("paymentStatus=Paid");
        qs.Should().Contain("search=B-001");
    }

    [Fact]
    public async Task GetAwaitingApprovalAsync_should_hit_awaiting_approval_endpoint()
    {
        var handler = TestHttpMessageHandler.RespondJson<IReadOnlyList<BillDto>>(Array.Empty<BillDto>());
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new BillsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<BillsApiClient>());

        await sut.GetAwaitingApprovalAsync();

        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be("/api/v1/bills/awaiting-approval");
    }

    [Fact]
    public async Task ApproveAsync_should_POST_no_body_to_approve_endpoint()
    {
        var handler = TestHttpMessageHandler.RespondJson(SampleBill());
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new BillsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<BillsApiClient>());

        var id = Guid.NewGuid();
        await sut.ApproveAsync(id);

        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be($"/api/v1/bills/{id}/approve");
        handler.Requests[0].Content.Should().BeNull();
    }

    [Fact]
    public async Task PayAsync_should_POST_payment_body()
    {
        var handler = TestHttpMessageHandler.RespondJson(SampleBill());
        var (http, token, correlation) = TestServices.BuildHttpStack(handler);
        var sut = new BillsApiClient(http, token.Object, correlation.Object, TestServices.NullLog<BillsApiClient>());

        var id = Guid.NewGuid();
        var pay = new PayBillRequest(new DateOnly(2026, 2, 1), new MoneyDto(100m, "USD"), "BankTransfer", "ref-1");

        await sut.PayAsync(id, pay);

        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be($"/api/v1/bills/{id}/payments");
        handler.RequestBodies[0].Should().Contain("\"paymentMethod\":\"BankTransfer\"");
    }
}
