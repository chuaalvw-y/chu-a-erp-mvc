using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using ChuA.ERP.Web.Mvc.Contracts.Common;

namespace ChuA.ERP.Web.Mvc.ViewModels;

/// <summary>Route-preserving view model for shared pagination UI.</summary>
public sealed class PagerViewModel
{
    private readonly Dictionary<string, string?> _routeValues;

    private PagerViewModel(IPagedResult page, Dictionary<string, string?> routeValues)
    {
        PageNumber = page.PageNumber;
        PageSize = page.PageSize;
        TotalPages = page.TotalPages;
        HasPreviousPage = page.HasPreviousPage;
        HasNextPage = page.HasNextPage;
        _routeValues = routeValues;
    }

    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }

    public static PagerViewModel FromQuery(IPagedResult page, IQueryCollection query)
    {
        var routeValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var (key, values) in query)
        {
            if (key.Equals("pageNumber", StringComparison.OrdinalIgnoreCase)
                || key.Equals("pageSize", StringComparison.OrdinalIgnoreCase)
                || StringValues.IsNullOrEmpty(values))
            {
                continue;
            }

            routeValues[key] = values.ToString();
        }

        return new PagerViewModel(page, routeValues);
    }

    public string UrlFor(int pageNumber)
    {
        var values = new Dictionary<string, string?>(_routeValues, StringComparer.OrdinalIgnoreCase)
        {
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = PageSize.ToString()
        };

        return QueryHelpers.AddQueryString(string.Empty, values);
    }
}
