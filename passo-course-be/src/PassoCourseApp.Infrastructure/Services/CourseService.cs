using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.Infrastructure.Services;

public class CourseService(AppDbContext db) : ICourseService
{
    public async Task<CourseResponse> CreateAsync(CourseCreateRequest request, Guid instructorId)
    {
        var instructor = await db.Users.FindAsync(instructorId) ?? throw new InvalidOperationException("Instructor not found");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Difficulty = request.Difficulty,
            InstructorId = instructorId
        };

        foreach (var l in request.Lessons.OrderBy(x => x.Order))
        {
            course.Lessons.Add(new Lesson
            {
                Id = Guid.NewGuid(),
                Title = l.Title,
                Order = l.Order,
                CourseId = course.Id
            });
        }

        foreach (var q in request.Quizzes.OrderBy(x => x.Order))
        {
            course.Quizzes.Add(new Quiz
            {
                Id = Guid.NewGuid(),
                Title = q.Title,
                Order = q.Order,
                CourseId = course.Id
            });
        }

        course.TotalLessons = course.Lessons.Count;
        course.TotalQuizzes = course.Quizzes.Count;

        await db.Courses.AddAsync(course);
        await db.SaveChangesAsync();

        course.Instructor = instructor;

        return ToResponse(course);
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        var course = await db.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Lessons)
            .Include(x => x.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == id);

        return course is null ? null : ToResponse(course);
    }

    public async Task<IReadOnlyList<CourseResponse>> GetAllAsync()
    {
        var list = await db.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Lessons)
            .Include(x => x.Quizzes)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return list.Select(ToResponse).ToList();
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, CourseUpdateRequest request, Guid instructorId)
{
    await using var tx = await db.Database.BeginTransactionAsync();

    var course = await db.Courses
        .Include(c => c.Instructor)
        .Include(c => c.Lessons)
        .Include(c => c.Quizzes)
        .FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new KeyNotFoundException("Course not found");

    if (course.InstructorId != instructorId)
        throw new UnauthorizedAccessException("Not course owner");

    if (request.Title is not null) course.Title = request.Title;
    if (request.Description is not null) course.Description = request.Description;
    if (request.Duration.HasValue) course.Duration = request.Duration.Value;
    if (request.Difficulty.HasValue) course.Difficulty = request.Difficulty.Value;

    if (request.Lessons is not null)
    {
        var incoming = request.Lessons.OrderBy(x => x.Order).ToList();
        if (incoming.Count == 0)
        {
            await db.Lessons.Where(l => l.CourseId == course.Id).ExecuteDeleteAsync();
        }
        else
        {
            var hasAnyId = incoming.Any(x => x.Id != Guid.Empty);
            if (!hasAnyId)
            {
                await db.Lessons.Where(l => l.CourseId == course.Id).ExecuteDeleteAsync();
                int order = 1;
                foreach (var l in incoming)
                    db.Lessons.Add(new Lesson { Id = Guid.NewGuid(), Title = l.Title, Order = order++, CourseId = course.Id });
            }
            else
            {
                var ids = incoming.Where(x => x.Id != Guid.Empty).Select(x => x.Id).ToHashSet();
                await db.Lessons.Where(l => l.CourseId == course.Id && !ids.Contains(l.Id)).ExecuteDeleteAsync();
                await db.Lessons.Where(l => l.CourseId == course.Id).ExecuteUpdateAsync(u => u.SetProperty(x => x.Order, x => x.Order + 1_000_000));
                var current = await db.Lessons.Where(l => l.CourseId == course.Id).ToDictionaryAsync(l => l.Id, l => l);
                int order = 1;
                foreach (var l in incoming)
                {
                    if (l.Id != Guid.Empty && current.TryGetValue(l.Id, out var ex))
                    {
                        ex.Title = l.Title;
                        ex.Order = order++;
                    }
                    else
                    {
                        db.Lessons.Add(new Lesson { Id = Guid.NewGuid(), Title = l.Title, Order = order++, CourseId = course.Id });
                    }
                }
            }
        }
        await db.SaveChangesAsync();
    }

    if (request.Quizzes is not null)
    {
        var incoming = request.Quizzes.OrderBy(x => x.Order).ToList();
        if (incoming.Count == 0)
        {
            await db.Quizzes.Where(q => q.CourseId == course.Id).ExecuteDeleteAsync();
        }
        else
        {
            var hasAnyId = incoming.Any(x => x.Id != Guid.Empty);
            if (!hasAnyId)
            {
                await db.Quizzes.Where(q => q.CourseId == course.Id).ExecuteDeleteAsync();
                int order = 1;
                foreach (var q in incoming)
                    db.Quizzes.Add(new Quiz { Id = Guid.NewGuid(), Title = q.Title, Order = order++, CourseId = course.Id });
            }
            else
            {
                var ids = incoming.Where(x => x.Id != Guid.Empty).Select(x => x.Id).ToHashSet();
                await db.Quizzes.Where(q => q.CourseId == course.Id && !ids.Contains(q.Id)).ExecuteDeleteAsync();
                await db.Quizzes.Where(q => q.CourseId == course.Id).ExecuteUpdateAsync(u => u.SetProperty(x => x.Order, x => x.Order + 1_000_000));
                var current = await db.Quizzes.Where(q => q.CourseId == course.Id).ToDictionaryAsync(q => q.Id, q => q);
                int order = 1;
                foreach (var q in incoming)
                {
                    if (q.Id != Guid.Empty && current.TryGetValue(q.Id, out var ex))
                    {
                        ex.Title = q.Title;
                        ex.Order = order++;
                    }
                    else
                    {
                        db.Quizzes.Add(new Quiz { Id = Guid.NewGuid(), Title = q.Title, Order = order++, CourseId = course.Id });
                    }
                }
            }
        }
        await db.SaveChangesAsync();
    }

    course.TotalLessons = await db.Lessons.CountAsync(l => l.CourseId == course.Id);
    course.TotalQuizzes = await db.Quizzes.CountAsync(q => q.CourseId == course.Id);

    await db.SaveChangesAsync();
    await tx.CommitAsync();

    return ToResponse(course);
}


    public async Task DeleteAsync(Guid id, Guid instructorId)
    {
        var course = await db.Courses.FindAsync(id) ?? throw new KeyNotFoundException("Course not found");
        if (course.InstructorId != instructorId) throw new UnauthorizedAccessException("Not course owner");
        db.Courses.Remove(course);
        await db.SaveChangesAsync();
    }

    public async Task<List<CourseResponse>> GetAllCoursesByInstructorId(Guid instructorId)
    {
        var list = await db.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Lessons)
            .Include(x => x.Quizzes)
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return list.Select(ToResponse).ToList();
    }

    private static CourseResponse ToResponse(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        Duration = c.Duration,
        Difficulty = c.Difficulty.ToString(),
        InstructorId = c.InstructorId,
        InstructorEmail = c.Instructor.Email,
        InstructorFullName = c.Instructor.FullName,
        CreatedAt = c.CreatedAt,
        TotalLessons = c.TotalLessons,
        TotalQuizzes = c.TotalQuizzes,
        Lessons = c.Lessons?.OrderBy(l => l.Order).Select(l => new ContentItemDto { Id = l.Id, Title = l.Title, Order = l.Order }).ToList() ?? [],
        Quizes = c.Quizzes?.OrderBy(q => q.Order).Select(q => new ContentItemDto { Id = q.Id, Title = q.Title, Order = q.Order }).ToList() ?? []
    };
}
