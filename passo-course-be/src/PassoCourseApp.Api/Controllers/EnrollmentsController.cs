using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;

namespace PassoCourseApp.Api.Controllers;

[ApiController]
[Route("enrollments")]
[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<EnrollmentResponse>> Enroll([FromBody] EnrollmentRequest req)
    {
        var resp = await enrollmentService.EnrollAsync( UserId(), req.CourseId);
        return Ok(resp);
    }
    
    [HttpDelete("{courseId:guid}")]
    [Authorize(Roles = "User")] 
    public async Task<IActionResult> Unenroll(Guid courseId)
    {
        await enrollmentService.UnenrollAsync( UserId(), courseId);
        return NoContent();
    }


    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> MyEnrollments()
    {
        var list = await enrollmentService.GetMyEnrollmentsAsync( UserId());
        return Ok(list);
    }


    private Guid UserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException();
    }
}