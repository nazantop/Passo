using PassoCourseApp.Domain.Entities;

namespace PassoCourseApp.Application.DTO;

public record CourseCreateRequest(
    string Title,
    string Description,
    int Duration,
    DifficultyLevel Difficulty,
    int TotalLessons,
    int TotalQuizzes,
    List<ContentCreateDto> Lessons,
    List<ContentCreateDto> Quizzes);


public class CourseUpdateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Duration { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public int? TotalLessons { get; set; }
    public int? TotalQuizzes { get; set; }
    public List<ContentItemDto> Lessons { get; set; }
    public List<ContentItemDto> Quizzes { get; set; }
}


public class CourseResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Duration { get; set; }
    public string Difficulty { get; set; } = "";
    public double Percent { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorEmail { get; set; } = "";
    public string InstructorFullName { get; set; } = "";
    public int TotalLessons { get; set; }
    public int TotalQuizzes { get; set; }
    public List<ContentItemDto> Lessons { get; set; } = [];
    public List<ContentItemDto> Quizes { get; set; } = [];
    public List<Guid> CompletedLessonIds { get; set; } = [];
    public List<int>  CompletedQuizIndices { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}