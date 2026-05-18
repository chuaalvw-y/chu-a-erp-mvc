using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Filters;

public class GlobalExceptionFilterTests
{
    [Fact]
    public void Should_render_Error_view_with_correlation_id()
    {
        var correlation = new Mock<ICorrelationIdAccessor>();
        correlation.Setup(c => c.GetOrCreate()).Returns("corr-9");
        var sut = new GlobalExceptionFilter(NullLogger<GlobalExceptionFilter>.Instance, correlation.Object);

        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        var ctx = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = new InvalidOperationException("boom"),
        };

        sut.OnException(ctx);

        ctx.ExceptionHandled.Should().BeTrue();
        var view = ctx.Result as ViewResult;
        view.Should().NotBeNull();
        view!.ViewName.Should().Be("Error");
        var model = view.ViewData.Model as ErrorViewModel;
        model.Should().NotBeNull();
        model!.CorrelationId.Should().Be("corr-9");
        model.StatusCode.Should().Be(500);
    }
}
