using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TaskManager.Api.Data;
using TaskManager.Api.Services;
using TaskManager.Api.Models;
using TaskManager.Api.Services.TagResults;
using TaskManager.Api.DTOs;
using TaskManager.Api.Tests.Helpers;

namespace TaskManager.Api.Tests.Services;

public class TagServiceFactTests
{
    private readonly TagService _service;
    private readonly AppDbContext _context;

    public TagServiceFactTests()
    {
        _context = TestDbContextFactory.Create();
        _service = new TagService(_context);
    }

    [Fact]
    public async Task CreateTagAsync_DuplicateName_ShouldReturnDuplicateName()
    {
        var existingTag = new Tag { Name = "Urgent" };

        _context.Tags.Add(existingTag);
        await _context.SaveChangesAsync();

        var newTagDto = new CreateTagDto
        {
            Name = "Urgent" // Duplicate name
        };

        var result = await _service.CreateTagAsync(newTagDto);
        Assert.Equal(CreateTagResult.DuplicateName, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTagAsync_InvalidName_ShouldReturnValidationFailed()
    {
        var newTagDto = new CreateTagDto
        {
            Name = "   " // Invalid name (whitespace)
        };

        var result = await _service.CreateTagAsync(newTagDto);
        Assert.Equal(CreateTagResult.ValidationFailed, result.Result);
        Assert.Null(result.Created);
    }

}
