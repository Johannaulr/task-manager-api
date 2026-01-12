using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProgressStatusBoolToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dueDate",
                table: "taskItems",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "isCompleted",
                table: "taskItems",
                newName: "Progress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "taskItems",
                newName: "dueDate");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "taskItems",
                newName: "isCompleted");
        }
    }
}
