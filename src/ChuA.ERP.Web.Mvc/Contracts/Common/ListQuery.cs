namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>
/// Common query parameters used by list pages: search text, sort key, paging.
/// API clients translate these into the query string the API understands today
/// (currently only <c>search</c>) and apply paging client-side until server paging exists.
/// </summary>
public class ListQuery
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;

    public ListQuery Normalize()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 25;
        if (PageSize > 200) PageSize = 200;
        return this;
    }
}
