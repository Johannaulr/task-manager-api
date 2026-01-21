using System;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs;

public class TaskQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public ICollection<int> Tags { get; set; } = [];
    public TagFilterMode Mode { get; set; } = TagFilterMode.Any;
}
