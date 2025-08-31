namespace PassoCourseApp.Domain.Entities;

public class UserLesson
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}