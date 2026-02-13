using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Infrastructure.Shared;

public class PagedList<T>
{
    internal PagedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken); //Could be impoved into one single call to DB.
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, page, pageSize, totalCount);
    }
}

public static class PagingExtensions
{
    public static Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return PagedList<T>.CreateAsync(query, page, pageSize, cancellationToken);
    }
}
