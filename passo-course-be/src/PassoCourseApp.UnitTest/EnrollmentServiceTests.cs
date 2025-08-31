using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Infrastructure.Services;
using PassoCourseApp.UnitTest.Helpers;

namespace PassoCourseApp.UnitTest;

public class EnrollmentServiceTests(DbFixture fx) : IClassFixture<DbFixture>
{
    private Infrastructure.Data.AppDbContext NewDb() => fx.CreateContext();
    private EnrollmentService NewSvc(Infrastructure.Data.AppDbContext db) => new EnrollmentService(db);
    private static (Domain.Entities.User user, PassoCourseApp.Domain.Entities.User inst, Domain.Entities.Course course)
        Seed(PassoCourseApp.Infrastructure.Data.AppDbContext db) => MockData.SeedBasic(db);

    [Fact]
    public async Task Enroll_Succeeds_ThenDuplicate_Throws()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var svc = NewSvc(db);
        var res = await svc.EnrollAsync(user.Id, course.Id);
        Assert.Equal(course.Id, res.CourseId);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.EnrollAsync(user.Id, course.Id));
    }

    [Fact]
    public async Task Unenroll_RemovesEnrollment_AndNotFound_Throws()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var svc = NewSvc(db);
        await svc.EnrollAsync(user.Id, course.Id);
        await svc.UnenrollAsync(user.Id, course.Id);
        var exists = await db.Enrollments.AnyAsync(e => e.UserId == user.Id && e.CourseId == course.Id);
        Assert.False(exists);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.UnenrollAsync(user.Id, course.Id));
    }

    [Fact]
    public async Task GetMyEnrollments_ReturnsProjectedCourses()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var svc = NewSvc(db);
        await svc.EnrollAsync(user.Id, course.Id);
        var list = await svc.GetMyEnrollmentsAsync(user.Id);
        Assert.Single(list);
        Assert.Equal(course.Id, list[0].Id);
        Assert.False(string.IsNullOrWhiteSpace(list[0].InstructorEmail));
    }
}
