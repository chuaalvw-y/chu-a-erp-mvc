namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>
/// UI paging envelope. The API currently returns <c>IReadOnlyList&lt;T&gt;</c> from list
/// endpoints; the MVC wraps responses in this type to support client-side pagination,
/// search and sort. When the API gains server-side paging, the same shape will line up.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int PageNumber, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedResult<T> Empty(int pageSize = 25) =>
        new(Array.Empty<T>(), 0, 1, pageSize);

    /// <summary>Builds a PagedResult from an already-fetched flat collection.</summary>
    public static PagedResult<T> FromCollection(IReadOnlyCollection<T> items, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 25;
        var paged = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
        return new PagedResult<T>(paged, items.Count, pageNumber, pageSize);
    }
}
