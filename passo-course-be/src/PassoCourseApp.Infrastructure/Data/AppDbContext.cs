using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Domain.Entities;

namespace PassoCourseApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Progress> Progresses => Set<Progress>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<UserLesson> UserLessons => Set<UserLesson>();
    public DbSet<UserQuiz> UserQuizzes => Set<UserQuiz>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.Email).IsRequired();
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.HasData(new Role { Id = 1, Name = "User" }, new Role { Id = 2, Name = "Instructor" });
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Role).WithMany(r => r.UserRoles).HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<Course>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired();
            b.HasOne(x => x.Instructor)
                .WithMany(i => i.CoursesTaught)
                .HasForeignKey(x => x.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Property(x => x.TotalLessons).HasDefaultValue(0);
            b.Property(x => x.TotalQuizzes).HasDefaultValue(0);
        });

        modelBuilder.Entity<Enrollment>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.Enrollments).HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Course).WithMany(c => c.Enrollments).HasForeignKey(x => x.CourseId);
        });

        modelBuilder.Entity<Progress>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.Progresses).HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Course).WithMany(c => c.Progresses).HasForeignKey(x => x.CourseId);
        });

        modelBuilder.Entity<Lesson>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired();
            b.HasOne(x => x.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.CourseId, x.Order }).IsUnique();
        });

        modelBuilder.Entity<Quiz>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired();
            b.HasOne(x => x.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.CourseId, x.Order }).IsUnique();
        });

        modelBuilder.Entity<UserLesson>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.LessonId }).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.UserLessons).HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId);
            b.HasOne(x => x.Lesson).WithMany().HasForeignKey(x => x.LessonId);
        });

        modelBuilder.Entity<UserQuiz>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.CourseId, x.QuizIndex }).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.UserQuizzes).HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId);
        });
    }
}