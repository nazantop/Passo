using PassoCourseApp.Application.DTO;

namespace PassoCourseApp.Application.IServices;

public interface ICourseService
{
    Task<CourseResponse> CreateAsync(CourseCreateRequest request, Guid instructorId);
    Task<CourseResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CourseResponse>> GetAllAsync();
    Task<CourseResponse> UpdateAsync(Guid id, CourseUpdateRequest request, Guid instructorId);
    Task DeleteAsync(Guid id, Guid instructorId);
    Task<List<CourseResponse>> GetAllCoursesByInstructorId(Guid instructorId);
}