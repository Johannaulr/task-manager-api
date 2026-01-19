using System;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using TaskManager.Api.Services.TagResults;
using TaskManager.Api.Services.TaskResults;

namespace TaskManager.Api.Services;

public interface ITagService
{
    Task<(CreateTagResult Result, TagDto? Created)> CreateTagAsync(CreateTagDto createTagDto);
    Task<DeleteTagResult> DeleteTagAsync(int id);
    Task<TagDto?> GetTagByIdAsync(int id);
}
