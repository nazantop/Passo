using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.Infrastructure.Services;

public class EnrollmentService(AppDbContext db) : IEnrollmentService
{
    public async Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId)
    {
        var exists = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (exists) throw new InvalidOperationException("Already enrolled");


        var e1 = new Domain.Entities.Enrollment { Id = Guid.NewGuid(), UserId = userId, CourseId = courseId };
        await db.Enrollments.AddAsync(e1);
        await db.SaveChangesAsync();
        return new EnrollmentResponse { CourseId = courseId, EnrolledAt = e1.EnrolledAt };
    }
    
    public async Task UnenrollAsync(Guid userId, Guid courseId)
    {
        var entity = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (entity == null) throw new InvalidOperationException("Enrollment not found");

        db.Enrollments.Remove(entity);
        await db.SaveChangesAsync();
    }
    
    public async Task<IReadOnlyList<CourseResponse>> GetMyEnrollmentsAsync(Guid userId)
    {
        var query = from e in db.Enrollments
            join c in db.Courses on e.CourseId equals c.Id
            join u in db.Users on c.InstructorId equals u.Id
            where e.UserId == userId
            orderby e.EnrolledAt descending
            select new CourseResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Duration = c.Duration,
                Difficulty = c.Difficulty.ToString(),
                InstructorId = u.Id,
                InstructorEmail = u.Email,
                CreatedAt = c.CreatedAt
            };
        return await query.ToListAsync();
    }
}