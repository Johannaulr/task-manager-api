namespace TaskManager.Api.Services.TaskResults;

public enum AddTagToTaskResult
{
    Success,
    TaskNotFound,
    TagNotFound,
    TagAlreadyAdded,
    ValidationFailed,
}
