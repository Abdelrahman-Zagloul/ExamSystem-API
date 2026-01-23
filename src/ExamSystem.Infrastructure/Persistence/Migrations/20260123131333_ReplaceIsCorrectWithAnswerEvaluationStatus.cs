using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsCorrectWithAnswerEvaluationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "StudentAnswers");

            migrationBuilder.AddColumn<int>(
                name: "EvaluationStatus",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluationStatus",
                table: "StudentAnswers");

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "StudentAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
