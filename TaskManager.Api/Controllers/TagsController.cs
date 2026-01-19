using System;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Api.Services.TagResults;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TagsController : ControllerBase
{
    // POST /api/tags
    // GET /api/tags
    // GET /api/tags/{id}
    // DELETE /api/tags/{id}

    private readonly ITagService _tagService;

    public TagsController (ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagDto>> CreateTagAsync([FromBody] CreateTagDto tagDto)
    {
        var (result, tag) = await _tagService.CreateTagAsync(tagDto);
        return result switch
        {
            CreateTagResult.Success => CreatedAtAction
            (
                nameof(GetTagByIdAsync),
                new {id = tag.Id}, 
                tag
            ),
            CreateTagResult.DuplicateName => Conflict("A tag with this name already exists"),
            CreateTagResult.ValidationFailed => BadRequest("Invalid tag data"),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TagDto>> GetTagByIdAsync(int id)
    {
        try
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                $"An unexpected error occurred. {e.Message}");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTagAsync(int id)
    {
        try
        {
            var tag = await _tagService.DeleteTagAsync(id);
            return tag switch
            {
                DeleteTagResult.Success => Ok(),
                DeleteTagResult.NotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }
}
