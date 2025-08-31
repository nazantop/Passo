namespace PassoCourseApp.Domain.Entities;

public enum DifficultyLevel { Beginner = 0, Intermediate = 1, Advanced = 2 }


public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Duration { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int TotalLessons { get; set; } = 0;
    public int TotalQuizzes { get; set; } = 0;
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public Guid InstructorId { get; set; }
    public User Instructor { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}