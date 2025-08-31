using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.UnitTest.Helpers;

public static class MockData
{
    public static (User user, User instructor, Course course) SeedBasic(AppDbContext db)
    {
        var user = new User { Id = Guid.NewGuid(), Email = "user@test.com", FirstName="U", LastName="S", PasswordHash="x" };
        var instructor = new User { Id = Guid.NewGuid(), Email = "inst@test.com", FirstName="I", LastName="S", PasswordHash="x" };
        var course = new Course { Id = Guid.NewGuid(), Title = "T", Description = "D", Duration = 10, Difficulty = DifficultyLevel.Beginner, InstructorId = instructor.Id, TotalLessons = 2, TotalQuizzes = 1 };
        var l1 = new Lesson { Id = Guid.NewGuid(), CourseId = course.Id, Title = "L1", Order = 1 };
        var l2 = new Lesson { Id = Guid.NewGuid(), CourseId = course.Id, Title = "L2", Order = 2 };
        var q1 = new Quiz { Id = Guid.NewGuid(), CourseId = course.Id, Title = "Q1", Order = 1 };
        db.Users.AddRange(user, instructor);
        db.Courses.Add(course);
        db.Lessons.AddRange(l1, l2);
        db.Quizzes.Add(q1);
        db.SaveChanges();
        return (user, instructor, course);
    }
}