// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>
/// Tests for the IsAjaxRequest / PartialOrView helpers — the single-source mechanism that
/// lets one action serve either a full page or a partial fragment.
/// </summary>
public class ReactiveRequestExtensionsTests
{
    private sealed class TestController : Controller { }

    private static TestController BuildController(string? requestedWith)
    {
        var http = new DefaultHttpContext();
        if (requestedWith is not null)
        {
            http.Request.Headers[ReactiveRequestExtensions.RequestedWithHeader] = requestedWith;
        }
        return new TestController
        {
            ControllerContext = new ControllerContext { HttpContext = http }
        };
    }

    [Fact]
    public void IsAjaxRequest_should_be_true_when_X_Requested_With_is_XMLHttpRequest()
    {
        var ctrl = BuildController(ReactiveRequestExtensions.XmlHttpRequestValue);
        ctrl.IsAjaxRequest().Should().BeTrue();
    }

    [Fact]
    public void IsAjaxRequest_should_be_false_for_normal_navigation()
    {
        var ctrl = BuildController(null);
        ctrl.IsAjaxRequest().Should().BeFalse();
    }

    [Fact]
    public void IsAjaxRequest_should_be_case_insensitive()
    {
        var ctrl = BuildController("xmlhttprequest");
        ctrl.IsAjaxRequest().Should().BeTrue();
    }

    [Fact]
    public void PartialOrView_should_return_PartialViewResult_for_ajax_requests()
    {
        var ctrl = BuildController(ReactiveRequestExtensions.XmlHttpRequestValue);

        var result = ctrl.PartialOrView("Foo", new object());

        result.Should().BeOfType<PartialViewResult>();
    }

    [Fact]
    public void PartialOrView_should_return_ViewResult_for_normal_navigation()
    {
        var ctrl = BuildController(null);

        var result = ctrl.PartialOrView("Foo", new object());

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void RequestedPartial_should_return_lowercased_header_value()
    {
        var http = new DefaultHttpContext();
        http.Request.Headers[ReactiveRequestExtensions.PartialHeader] = "ROWS";
        var ctrl = new TestController { ControllerContext = new ControllerContext { HttpContext = http } };

        ctrl.RequestedPartial().Should().Be("rows");
    }

    [Fact]
    public void RequestedPartial_should_return_null_when_header_missing()
    {
        var ctrl = BuildController(null);
        ctrl.RequestedPartial().Should().BeNull();
    }
}
