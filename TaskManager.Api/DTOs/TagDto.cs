namespace TaskManager.Api.DTOs;

public record class TagDto
{
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    
}
