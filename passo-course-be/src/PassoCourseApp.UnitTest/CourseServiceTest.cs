using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;
using PassoCourseApp.Infrastructure.Services;
using PassoCourseApp.UnitTest.Helpers;

namespace PassoCourseApp.UnitTest;

public class CourseServiceTests(DbFixture fx) : IClassFixture<DbFixture>
{
    private AppDbContext NewDb() => fx.CreateContext();
    private CourseService NewSvc(AppDbContext db) => new CourseService(db);
    private (User user, User inst, Course course) Seed(AppDbContext db) => MockData.SeedBasic(db);
    private static string U(string p) => $"{p}_{Guid.NewGuid():N}@test.local";
    private async Task<User> NewInstructor(AppDbContext db)
    {
        var inst = new User { Id = Guid.NewGuid(), Email = U("inst"), FirstName = "N", LastName = "S", PasswordHash = "x" };
        db.Users.Add(inst);
        await db.SaveChangesAsync();
        return inst;
    }

    [Fact]
    public async Task Create_PopulatesTotals_AndReturnsResponse()
    {
        await using var db = NewDb();
        var inst = await NewInstructor(db);
        var svc = NewSvc(db);

        var req = new CourseCreateRequest(
            Title: "English",
            Description: "Desc",
            Duration: 12,
            Difficulty: DifficultyLevel.Beginner,
            Lessons:
            [
                new ContentCreateDto { Title = "Intro",  Order = 1 },
                new ContentCreateDto { Title = "Basics", Order = 2 }
            ],
            Quizzes: [ new ContentCreateDto { Title = "Q1", Order = 1 } ],
            TotalLessons: 2,
            TotalQuizzes: 1
        );

        var res = await svc.CreateAsync(req, inst.Id);
        var saved = await db.Courses.Include(c => c.Lessons).Include(c => c.Quizzes).FirstAsync(c => c.Id == res.Id);

        Assert.Equal(2, saved.TotalLessons);
        Assert.Equal(1, saved.TotalQuizzes);
        Assert.Equal(2, res.TotalLessons);
        Assert.Equal(1, res.TotalQuizzes);
        Assert.Equal("Intro", res.Lessons.First().Title);
    }

    [Fact]
    public async Task GetById_ReturnsWithContent()
    {
        await using var db = NewDb();
        var (_, _, course) = Seed(db);
        var svc = NewSvc(db);
        var res = await svc.GetByIdAsync(course.Id);
        Assert.NotNull(res);
        Assert.Equal(course.Id, res!.Id);
        Assert.True(res.Lessons.Count >= 1);
        Assert.Equal(course.TotalQuizzes, res.TotalQuizzes);
    }

    [Fact]
    public async Task Update_ModifiesLessons_AndTotals()
    {
        await using var db = NewDb();
        var (_, inst, course) = Seed(db);
        var svc = NewSvc(db);

        var currentLessons = await db.Lessons.Where(l => l.CourseId == course.Id).OrderBy(l => l.Order).ToListAsync();
        var upd = new CourseUpdateRequest
        {
            Title = "NewT",
            Lessons =
            [
                new ContentItemDto { Id = currentLessons[0].Id, Title = "L1x", Order = 1 },
                new ContentItemDto { Title = "L3", Order = 2 }
            ],
            Quizzes =
            [
                new ContentItemDto { Title = "Q1x", Order = 1 }
            ]
        };

        var res = await svc.UpdateAsync(course.Id, upd, inst.Id);
        var savedLessons = await db.Lessons.Where(l => l.CourseId == course.Id).OrderBy(l => l.Order).ToListAsync();

        Assert.Equal("NewT", res.Title);
        Assert.Equal(2, savedLessons.Count);
        Assert.Equal(1, await db.Quizzes.CountAsync(q => q.CourseId == course.Id));
        Assert.Equal(2, res.TotalLessons);
        Assert.Equal(1, res.TotalQuizzes);
    }

    [Fact]
    public async Task Update_Unauthorized_Throws()
    {
        await using var db = NewDb();
        var (_, _, course) = Seed(db);
        var svc = NewSvc(db);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            svc.UpdateAsync(course.Id, new CourseUpdateRequest { Title = "X" }, Guid.NewGuid()));
    }

    [Fact]
    public async Task Delete_RemovesCourse_WhenOwner()
    {
        await using var db = NewDb();
        var (_, inst, course) = Seed(db);
        var svc = NewSvc(db);
        await svc.DeleteAsync(course.Id, inst.Id);
        var exists = await db.Courses.AnyAsync(c => c.Id == course.Id);
        Assert.False((bool)exists);
    }
}
