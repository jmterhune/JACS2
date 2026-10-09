namespace CourtCounsel.Desktop.Data;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public long TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalItems / (double)PageSize);
}
