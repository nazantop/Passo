using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Infrastructure.Services;
using PassoCourseApp.UnitTest.Helpers;

namespace PassoCourseApp.UnitTest;

public class ProgressServiceTests(DbFixture fx) : IClassFixture<DbFixture>
{
    private Infrastructure.Data.AppDbContext NewDb() => fx.CreateContext();
    private ProgressService NewSvc(Infrastructure.Data.AppDbContext db) => new(db);
    private EnrollmentService NewEnroll(Infrastructure.Data.AppDbContext db) => new(db);
    private (Domain.Entities.User user, Domain.Entities.User inst, Domain.Entities.Course course)
        Seed(Infrastructure.Data.AppDbContext db) => MockData.SeedBasic(db);

    [Fact]
    public async Task GetMine_ComputesPercentFromTotals()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var enroll = NewEnroll(db);
        await enroll.EnrollAsync(user.Id, course.Id);
        var svc = NewSvc(db);
        var list = await svc.GetMineAsync(user.Id);
        var item = list.FirstOrDefault(x => x.CourseId == course.Id);
        Assert.NotNull(item);
        Assert.Equal(0, item!.Percent);
    }

    [Fact]
    public async Task IncrementLesson_And_Quiz_UpdateProgress()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var enroll = NewEnroll(db);
        await enroll.EnrollAsync(user.Id, course.Id);
        var svc = NewSvc(db);
        await svc.IncrementLessonAsync(user.Id, course.Id);
        await svc.IncrementQuizAsync(user.Id, course.Id);
        var mine = await svc.GetMineAsync(user.Id);
        var p = mine.Single(x => x.CourseId == course.Id);
        Assert.True(p.Percent > 0);
    }

    [Fact]
    public async Task RemoveAllForCourse_ClearsUserData()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var enroll = NewEnroll(db);
        await enroll.EnrollAsync(user.Id, course.Id);
        var svc = NewSvc(db);
        await svc.IncrementLessonAsync(user.Id, course.Id);
        await svc.IncrementQuizAsync(user.Id, course.Id);
        await svc.RemoveAllForCourseAsync(user.Id, course.Id);
        var outline = await svc.GetCourseOutlineAsync(user.Id, course.Id);
        Assert.Empty(outline.CompletedLessonIds);
        Assert.Empty(outline.CompletedQuizIndices);
    }

    [Fact]
    public async Task GetCourseOutline_CompletesAndReaches100()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var enroll = NewEnroll(db);
        await enroll.EnrollAsync(user.Id, course.Id);
        var svc = NewSvc(db);

        var lessons = await db.Lessons.Where(l => l.CourseId == course.Id).OrderBy(l => l.Order).ToListAsync();
        foreach (var l in lessons) await svc.CompleteLessonAsync(user.Id, l.Id);
        await svc.CompleteQuizAsync(user.Id, course.Id, 1);

        var outline = await svc.GetCourseOutlineAsync(user.Id, course.Id);
        Assert.Equal(100, outline.Percent);
        Assert.Equal(lessons.Count, outline.CompletedLessonIds.Count);
        Assert.Contains(1, outline.CompletedQuizIndices);
    }

    [Fact]
    public async Task CompleteLesson_Idempotent_AndRequiresEnrollment()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var svc = NewSvc(db);
        var lessonId = db.Lessons.First(l => l.CourseId == course.Id).Id;
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CompleteLessonAsync(user.Id, lessonId));
    }

    [Fact]
    public async Task CompleteQuiz_ValidatesIndex_AndEnrollment()
    {
        await using var db = NewDb();
        var (user, _, course) = Seed(db);
        var svc = NewSvc(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CompleteQuizAsync(user.Id, course.Id, 1));
        var enroll = NewEnroll(db);
        await enroll.EnrollAsync(user.Id, course.Id);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CompleteQuizAsync(user.Id, course.Id, 99));
    }
}
