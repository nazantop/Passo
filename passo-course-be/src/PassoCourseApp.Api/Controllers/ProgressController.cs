using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;

namespace PassoCourseApp.Api.Controllers;

[ApiController]
[Route("progress")]
[Authorize]
public class ProgressController(IProgressService progressService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<List<ProgressResponse>>> GetMine()
        => Ok(await progressService.GetMineAsync(UserId()));

    [HttpPost("lesson-completed")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> LessonCompleted([FromBody] IncrementRequest req)
    {
        await progressService.IncrementLessonAsync(UserId(), req.CourseId); 
        return NoContent();
    }

    [HttpPost("quiz-completed")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> QuizCompleted([FromBody] IncrementRequest req)
    {
        await progressService.IncrementQuizAsync(UserId(), req.CourseId);
        return NoContent();
    }

    [HttpPost("lessons/{lessonId:guid}/complete")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId)
    {
        await progressService.CompleteLessonAsync(UserId(), lessonId);
        return NoContent();
    }

    [HttpPost("quizzes/{courseId:guid}/{quizIndex:int}/complete")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CompleteQuiz(Guid courseId, int quizIndex)
    {
        await progressService.CompleteQuizAsync(UserId(), courseId, quizIndex); 
        return NoContent();
    }
    private Guid UserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException();
    }
}