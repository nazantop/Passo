using Microsoft.Extensions.Caching.Distributed;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Infrastructure.Caching;
using PassoCourseApp.Infrastructure.Extensions;

namespace PassoCourseApp.Infrastructure.Services;

public class CachedProgressService(IProgressService inner, IDistributedCache cache, ICacheGate gate)
    : IProgressService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public Task<List<ProgressResponse>> GetMineAsync(Guid userId)
        => cache.GetOrSetAsync(gate, $"progress:{userId}", () => inner.GetMineAsync(userId), Ttl)!;

    public Task<CourseResponse> GetCourseOutlineAsync(Guid userId, Guid courseId)
        => cache.GetOrSetAsync(gate, $"outline:{courseId}:{userId}", () => inner.GetCourseOutlineAsync(userId, courseId), Ttl)!;

    public async Task IncrementLessonAsync(Guid userId, Guid courseId)
    {
        await inner.IncrementLessonAsync(userId, courseId);
        await cache.RemoveMany(gate, $"progress:{userId}", $"outline:{courseId}:{userId}");
    }

    public async Task IncrementQuizAsync(Guid userId, Guid courseId)
    {
        await inner.IncrementQuizAsync(userId, courseId);
        await cache.RemoveMany(gate, $"progress:{userId}", $"outline:{courseId}:{userId}");
    }

    public async Task RemoveAllForCourseAsync(Guid userId, Guid courseId)
    {
        await inner.RemoveAllForCourseAsync(userId, courseId);
        await cache.RemoveMany(gate, $"progress:{userId}", $"outline:{courseId}:{userId}");
    }

    public async Task CompleteLessonAsync(Guid userId, Guid lessonId)
    {
        await inner.CompleteLessonAsync(userId, lessonId);
        await cache.RemoveMany(gate, $"progress:{userId}");
    }

    public async Task CompleteQuizAsync(Guid userId, Guid courseId, int quizIndex)
    {
        await inner.CompleteQuizAsync(userId, courseId, quizIndex);
        await cache.RemoveMany(gate, $"progress:{userId}", $"outline:{courseId}:{userId}");
    }
}
