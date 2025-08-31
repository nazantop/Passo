using PassoCourseApp.Application.DTO;

namespace PassoCourseApp.Application.IServices;

public interface IProgressService
{
    Task<List<ProgressResponse>> GetMineAsync(Guid userId);
    Task IncrementLessonAsync(Guid userId, Guid courseId);
    Task IncrementQuizAsync(Guid userId, Guid courseId);
    Task RemoveAllForCourseAsync(Guid userId, Guid courseId);
    Task<CourseResponse> GetCourseOutlineAsync(Guid userId, Guid courseId);
    Task CompleteQuizAsync(Guid userId, Guid courseId, int quizIndex);
    Task CompleteLessonAsync(Guid userId, Guid lessonId);
}