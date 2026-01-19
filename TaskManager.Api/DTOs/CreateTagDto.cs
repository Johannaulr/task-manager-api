namespace TaskManager.Api.DTOs;

public record class CreateTagDto
{
    public string Name {get; set;} = string.Empty;
}
