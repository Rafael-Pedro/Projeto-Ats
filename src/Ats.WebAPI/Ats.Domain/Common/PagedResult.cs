namespace Ats.Domain.Common;

public record PagedResult<T>(IEnumerable<T> Items, long TotalCount, int Page, int PageSize);
