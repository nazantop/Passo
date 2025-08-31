namespace PassoCourseApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; }
    public string LastName  { get; set; }
    public string FullName  => $"{FirstName} {LastName}".Trim();
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;


    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    public ICollection<UserLesson> UserLessons { get; set; } = new List<UserLesson>();
    public ICollection<UserQuiz> UserQuizzes { get; set; } = new List<UserQuiz>();
}