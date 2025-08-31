using PassoCourseApp.Application.DTO;

namespace PassoCourseApp.Application.IServices;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId);
    Task UnenrollAsync(Guid userId, Guid courseId);
    Task<IReadOnlyList<CourseResponse>> GetMyEnrollmentsAsync(Guid userId);
}