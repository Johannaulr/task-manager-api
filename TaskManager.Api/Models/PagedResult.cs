using System;

namespace TaskManager.Api.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Tasks { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
