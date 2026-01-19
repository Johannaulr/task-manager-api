using System;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services.TaskResults;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services.TagResults;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;
    public TagService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<(CreateTagResult Result, TagDto? Created)> CreateTagAsync(CreateTagDto createTagDto)
    {
        // Business rule: duplicate name
        bool exists = await _context.Tags
            .AnyAsync(tag => tag.Name == createTagDto.Name);

        if (exists)
            return (CreateTagResult.DuplicateName, null);

        if (string.IsNullOrWhiteSpace(createTagDto.Name))
            return (CreateTagResult.ValidationFailed, null); 

        var tag = new Tag
        {
            Name = createTagDto.Name
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var tagDto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };

        return (CreateTagResult.Success, tagDto);
    }

    // READ
    public async Task<TagDto> GetTagByIdAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return null;
        
        var tagDto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };

        return tagDto;
    }

    // DELETE
    public async Task<DeleteTagResult> DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return DeleteTagResult.NotFound;
        
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return DeleteTagResult.Success;
    }
}
