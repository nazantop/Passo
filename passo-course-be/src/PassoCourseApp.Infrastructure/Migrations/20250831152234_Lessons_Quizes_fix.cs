using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassoCourseApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Lessons_Quizes_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId_Order",
                table: "Quizzes",
                columns: new[] { "CourseId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId_Order",
                table: "Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");
        }
    }
}
