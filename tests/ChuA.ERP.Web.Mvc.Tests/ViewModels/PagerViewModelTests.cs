using ChuA.ERP.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ChuA.ERP.Web.Mvc.Tests.ViewModels;

public class PagerViewModelTests
{
    [Fact]
    public void UrlFor_should_preserve_existing_filters_and_replace_page_values()
    {
        var page = new PagedResult<string>(Array.Empty<string>(), 100, 3, 25);
        var query = new QueryCollection(new Dictionary<string, StringValues>
        {
            ["search"] = "acme parts",
            ["status"] = "Open",
            ["vendorId"] = Guid.Parse("11111111-1111-1111-1111-111111111111").ToString(),
            ["pageNumber"] = "3",
            ["pageSize"] = "50"
        });

        var pager = PagerViewModel.FromQuery(page, query);

        var url = pager.UrlFor(4);

        url.Should().Contain("search=acme%20parts");
        url.Should().Contain("status=Open");
        url.Should().Contain("vendorId=11111111-1111-1111-1111-111111111111");
        url.Should().Contain("pageNumber=4");
        url.Should().Contain("pageSize=25");
        url.Should().NotContain("pageNumber=3");
        url.Should().NotContain("pageSize=50");
    }
}
