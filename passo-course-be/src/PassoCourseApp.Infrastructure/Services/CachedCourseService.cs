using Microsoft.Extensions.Caching.Distributed;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Infrastructure.Caching;
using PassoCourseApp.Infrastructure.Extensions;

namespace PassoCourseApp.Infrastructure.Services;

public class CachedCourseService(ICourseService inner, IDistributedCache cache, ICacheGate gate)
    : ICourseService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public Task<IReadOnlyList<CourseResponse>> GetAllAsync()
        => cache.GetOrSetAsync(gate, "courses:all", () => inner.GetAllAsync(), Ttl)!;

    public Task<CourseResponse?> GetByIdAsync(Guid id)
        => cache.GetOrSetAsync(gate, $"courses:{id}", () => inner.GetByIdAsync(id), Ttl)!;

    public async Task<CourseResponse> CreateAsync(CourseCreateRequest request, Guid instructorId)
    {
        var r = await inner.CreateAsync(request, instructorId);
        await cache.RemoveMany(gate, "courses:all");
        return r;
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, CourseUpdateRequest request, Guid instructorId)
    {
        var r = await inner.UpdateAsync(id, request, instructorId);
        await cache.RemoveMany(gate, "courses:all", $"courses:{id}");
        return r;
    }

    public async Task DeleteAsync(Guid id, Guid instructorId)
    {
        await inner.DeleteAsync(id, instructorId);
        await cache.RemoveMany(gate, "courses:all", $"courses:{id}");
    }

    public Task<List<CourseResponse>> GetAllCoursesByInstructorId(Guid instructorId)
        => cache.GetOrSetAsync(gate, $"instructor:{instructorId}:courses", () => inner.GetAllCoursesByInstructorId(instructorId), Ttl)!;
}