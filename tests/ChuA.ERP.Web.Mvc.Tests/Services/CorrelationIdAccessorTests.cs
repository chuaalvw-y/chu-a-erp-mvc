// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using Microsoft.AspNetCore.Http;

namespace ChuA.ERP.Web.Mvc.Tests.Services;

public class CorrelationIdAccessorTests
{
    [Fact]
    public void Returns_inbound_header_when_present()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers[ApiHeaders.CorrelationId] = "abc-123";
        var accessor = new HttpContextAccessor { HttpContext = ctx };

        var sut = new CorrelationIdAccessor(accessor);

        sut.GetOrCreate().Should().Be("abc-123");
    }

    [Fact]
    public void Falls_back_to_trace_identifier_when_no_header()
    {
        var ctx = new DefaultHttpContext { TraceIdentifier = "trace-xyz" };
        var accessor = new HttpContextAccessor { HttpContext = ctx };

        var sut = new CorrelationIdAccessor(accessor);

        sut.GetOrCreate().Should().Be("trace-xyz");
    }

    [Fact]
    public void Caches_the_value_on_HttpContext_Items()
    {
        var ctx = new DefaultHttpContext { TraceIdentifier = "first" };
        var accessor = new HttpContextAccessor { HttpContext = ctx };
        var sut = new CorrelationIdAccessor(accessor);

        var first = sut.GetOrCreate();
        ctx.TraceIdentifier = "second";
        var second = sut.GetOrCreate();

        first.Should().Be(second);
    }

    [Fact]
    public void Generates_new_guid_when_no_http_context()
    {
        var accessor = new HttpContextAccessor { HttpContext = null };
        var sut = new CorrelationIdAccessor(accessor);

        sut.GetOrCreate().Should().NotBeNullOrEmpty();
    }
}
