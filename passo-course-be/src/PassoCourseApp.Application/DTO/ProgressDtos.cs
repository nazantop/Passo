namespace PassoCourseApp.Application.DTO;

public class ProgressResponse
{
    public Guid CourseId { get; set; }
    public int CompletedLessons { get; set; }
    public int CompletedQuizzes { get; set; }
    public int TotalLessons { get; set; }
    public int TotalQuizzes { get; set; }
    public double Percent { get; set; } 
}

public class IncrementRequest
{
    public Guid CourseId { get; set; }
}