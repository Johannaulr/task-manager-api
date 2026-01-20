namespace TaskManager.Api.Services.TagResults;

public enum CreateTagResult
{
    Success,
    ValidationFailed, // Tag data is invalid
    DuplicateName // Tag name already exists
}
