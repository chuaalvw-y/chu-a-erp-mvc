using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace ChuA.ERP.Web.Mvc.Tests.Filters;

public class CorrelationIdActionFilterTests
{
    [Fact]
    public async Task Adds_correlation_id_header_to_response()
    {
        var correlation = new Mock<ICorrelationIdAccessor>();
        correlation.Setup(c => c.GetOrCreate()).Returns("corr-42");

        var sut = new CorrelationIdActionFilter(correlation.Object);

        var ctx = new DefaultHttpContext();
        var actionContext = new ActionContext(ctx, new RouteData(), new ActionDescriptor());
        var executing = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), this);
        var nextCalled = false;
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), this));
        }

        await sut.OnActionExecutionAsync(executing, Next);

        nextCalled.Should().BeTrue();
        ctx.Response.Headers[ApiHeaders.CorrelationId].ToString().Should().Be("corr-42");
    }
}
