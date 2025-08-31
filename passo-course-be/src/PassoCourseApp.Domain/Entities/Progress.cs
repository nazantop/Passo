namespace PassoCourseApp.Domain.Entities;

public class Progress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public int CompletedLessons { get; set; }
    public int CompletedQuizzes { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = default!;
    public Course Course { get; set; } = default!;
}