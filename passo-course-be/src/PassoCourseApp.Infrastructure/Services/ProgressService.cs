using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.Infrastructure.Services;

public class ProgressService(AppDbContext db) : IProgressService
{
    public async Task<List<ProgressResponse>> GetMineAsync(Guid userId)
    {
        var enrolledIds = await db.Enrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.CourseId)
            .ToListAsync();

        var totals = await db.Courses
            .Where(c => enrolledIds.Contains(c.Id))
            .Select(c => new {
                c.Id,
                TotalLessons = db.Lessons.Count(l => l.CourseId == c.Id),
                TotalQuizzes = db.Quizzes.Count(q => q.CourseId == c.Id)
            })
            .ToListAsync();

        var prog = await db.Progresses
            .Where(p => p.UserId == userId && enrolledIds.Contains(p.CourseId))
            .ToListAsync();

        var list = new List<ProgressResponse>();
        foreach (var t in totals)
        {
            var p = prog.FirstOrDefault(x => x.CourseId == t.Id);
            var cl = p?.CompletedLessons ?? 0;
            var cq = p?.CompletedQuizzes ?? 0;
            var denom = Math.Max(1, t.TotalLessons + t.TotalQuizzes);
            list.Add(new ProgressResponse {
                CourseId = t.Id,
                CompletedLessons = cl,
                CompletedQuizzes = cq,
                TotalLessons = t.TotalLessons,
                TotalQuizzes = t.TotalQuizzes,
                Percent = Math.Round((cl + cq) * 100.0 / denom, 0)
            });
        }
        return list;
    }

    public async Task IncrementLessonAsync(Guid userId, Guid courseId)
    {
        var enrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (!enrolled) throw new InvalidOperationException("Not enrolled");

        var p = await db.Progresses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        if (p == null)
            db.Progresses.Add(new Progress { UserId = userId, CourseId = courseId, CompletedLessons = 1 });
        else { p.CompletedLessons++; p.LastUpdated = DateTime.UtcNow; }
        await db.SaveChangesAsync();
    }

    public async Task IncrementQuizAsync(Guid userId, Guid courseId)
    {
        var enrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (!enrolled) throw new InvalidOperationException("Not enrolled");

        var p = await db.Progresses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        if (p == null)
            db.Progresses.Add(new Progress { UserId = userId, CourseId = courseId, CompletedQuizzes = 1 });
        else { p.CompletedQuizzes++; p.LastUpdated = DateTime.UtcNow; }
        await db.SaveChangesAsync();
    }

    public async Task RemoveAllForCourseAsync(Guid userId, Guid courseId)
    {
        var ul = db.UserLessons.Where(x => x.UserId == userId && x.CourseId == courseId);
        var uq = db.UserQuizzes.Where(x => x.UserId == userId && x.CourseId == courseId);
        db.UserLessons.RemoveRange(ul);
        db.UserQuizzes.RemoveRange(uq);
        var p = await db.Progresses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        if (p != null) db.Progresses.Remove(p);
        await db.SaveChangesAsync();
    }
    
    public async Task<CourseResponse> GetCourseOutlineAsync(Guid userId, Guid courseId)
    {
        var c = await db.Courses
            .Include(x => x.Lessons).Include(course => course.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == courseId);
        if (c == null) throw new KeyNotFoundException("Course not found");

        var completedLessons = await db.UserLessons
            .Where(x => x.UserId == userId && x.CourseId == courseId)
            .Select(x => x.LessonId)
            .ToListAsync();

        var completedQuizzes = await db.UserQuizzes
            .Where(x => x.UserId == userId && x.CourseId == courseId)
            .Select(x => x.QuizIndex)
            .ToListAsync();

        var completedCount = completedLessons.Count + completedQuizzes.Count;
        var total = Math.Max(1, c.TotalLessons + c.TotalQuizzes);
        var percent = Math.Round(completedCount * 100.0 / total, 0);

        return new CourseResponse {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Duration = c.Duration,
            Difficulty = c.Difficulty.ToString(),
            TotalLessons = c.TotalLessons,
            TotalQuizzes = c.TotalQuizzes,
            Quizes = c.Quizzes.OrderBy(l => l.Order)
                .Select(l => new ContentItemDto() { Id = l.Id, Title = l.Title, Order = l.Order }).ToList(),

            Lessons = c.Lessons.OrderBy(l => l.Order)
                .Select(l => new ContentItemDto() { Id = l.Id, Title = l.Title, Order = l.Order }).ToList(),
            CompletedLessonIds = completedLessons,
            CompletedQuizIndices = completedQuizzes,
            Percent = percent
        };
    }
    
    public async Task CompleteLessonAsync(Guid userId, Guid lessonId)
    {
        var lesson = await db.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null) throw new KeyNotFoundException("Lesson not found");

        var enrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);
        if (!enrolled) throw new InvalidOperationException("Not enrolled");

        var exists = await db.UserLessons.AnyAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);
        if (exists) return;

        db.UserLessons.Add(new UserLesson { UserId = userId, CourseId = lesson.CourseId, LessonId = lessonId });
        var p = await db.Progresses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == lesson.CourseId);
        if (p == null) db.Progresses.Add(new Progress { UserId = userId, CourseId = lesson.CourseId, CompletedLessons = 1 });
        else { p.CompletedLessons++; p.LastUpdated = DateTime.UtcNow; }
        await db.SaveChangesAsync();
    }
    
    public async Task CompleteQuizAsync(Guid userId, Guid courseId, int quizIndex)
    {
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");
        if (quizIndex < 1 || quizIndex > course.TotalQuizzes) throw new InvalidOperationException("Invalid quiz index");

        var enrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (!enrolled) throw new InvalidOperationException("Not enrolled");

        var exists = await db.UserQuizzes.AnyAsync(uq => uq.UserId == userId && uq.CourseId == courseId && uq.QuizIndex == quizIndex);
        if (exists) return;

        db.UserQuizzes.Add(new UserQuiz { UserId = userId, CourseId = courseId, QuizIndex = quizIndex });
        var p = await db.Progresses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        if (p == null) db.Progresses.Add(new Progress { UserId = userId, CourseId = courseId, CompletedQuizzes = 1 });
        else { p.CompletedQuizzes++; p.LastUpdated = DateTime.UtcNow; }
        await db.SaveChangesAsync();
    }
}