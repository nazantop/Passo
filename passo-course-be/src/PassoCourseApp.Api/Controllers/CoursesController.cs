using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;

namespace PassoCourseApp.Api.Controllers;

[ApiController]
[Route("courses")]
public class CoursesController(ICourseService courseService, IProgressService progressService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        => Ok(await courseService.GetAllAsync());


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseResponse>> GetById([FromRoute] Guid id)
    {
        var course = await courseService.GetByIdAsync(id);
        return course is null ? NotFound() : Ok(course);
    }


    [Authorize(Roles = "Instructor")]
    [HttpPost]
    public async Task<ActionResult<CourseResponse>> Create([FromBody] CourseCreateRequest req)
    {
        var instructorId = UserId();
        var created = await courseService.CreateAsync(req, instructorId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


    [Authorize(Roles = "Instructor")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CourseResponse>> Update([FromRoute] Guid id, [FromBody] CourseUpdateRequest req)
    {
        var instructorId = UserId();
        var updated = await courseService.UpdateAsync(id, req, instructorId);
        return Ok(updated);
    }


    [Authorize(Roles = "Instructor")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var instructorId = UserId();
        await courseService.DeleteAsync(id, instructorId);
        return NoContent();
    }

    [HttpGet("{id:guid}/outline")]
    [Authorize]
    public async Task<ActionResult<CourseResponse>> Outline(Guid id)
        => Ok(await progressService.GetCourseOutlineAsync(UserId(), id));

    [HttpGet("my-courses")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAllCoursesByInstructorId(Guid id)
        => Ok(await courseService.GetAllCoursesByInstructorId(UserId()));

    private Guid UserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException();
    }
}