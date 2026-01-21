using System;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Services;
using TaskManager.Api.Controllers;
using TaskManager.Api.Tests.Helpers;
using TaskManager.Api.Services.TaskResults;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using Microsoft.AspNetCore.Http;

namespace TaskManager.Api.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _controller = new TasksController(_serviceMock.Object);
    }

    // GetAllTasks Pagination Tests

    [Theory]
    [InlineData(0, 10, 0, typeof(BadRequestObjectResult))]
    [InlineData(1, 0, 0, typeof(BadRequestObjectResult))]
    [InlineData(1, 999, 0, typeof(BadRequestObjectResult))]
    [InlineData(-1, 10, 0, typeof(BadRequestObjectResult))]
    [InlineData(1, -5, 0, typeof(BadRequestObjectResult))]
    [InlineData(1, 10, 0, typeof(NoContentResult))]
    [InlineData(1, 10, 1, typeof(OkObjectResult))]
    public async Task GetAllTasks_ReturnsExpectedResultType(int pageNumber, int pageSize, int totalCount, Type expectedType)
    {
        _serviceMock.Setup(s => s.GetAllTasksAsync(It.Is<TaskQueryDto>(q => q.Page == pageNumber && q.PageSize == pageSize)))
                    .ReturnsAsync(new PagedResult<TaskDto> { Tasks = new List<TaskDto>(), TotalCount = totalCount });
        
        var queryDto = new TaskQueryDto
        {
            Page = pageNumber,
            PageSize = pageSize
        };

        var result = await _controller.GetAllTasks(queryDto);

        Assert.IsType(expectedType, result.Result);
    }


    [Fact]
    public async Task GetAllTasks_ServiceThrowsException_ShouldReturnInternalServerError()
    {
        _serviceMock
            .Setup(s => s.GetAllTasksAsync(It.IsAny<TaskQueryDto>()))
            .ThrowsAsync(new Exception("Database error"));

        var queryDto = new TaskQueryDto
        {
            Page = 1,
            PageSize = 10
        };

        var result = await _controller.GetAllTasks(queryDto);

        var internalError = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, internalError.StatusCode);
    }
}
