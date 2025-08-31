namespace PassoCourseApp.Application.DTO;

public record EnrollmentRequest(Guid CourseId);
public class EnrollmentResponse
{
    public Guid CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
}