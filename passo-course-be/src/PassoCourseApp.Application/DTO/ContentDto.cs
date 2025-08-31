namespace PassoCourseApp.Application.DTO;

public class ContentCreateDto
{
    public string Title { get; set; } = "";
    public int Order { get; set; }
}

public class ContentItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public int Order { get; set; }
}