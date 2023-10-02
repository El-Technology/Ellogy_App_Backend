using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AICommunicationService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFunctionsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Functions",
                table: "AIPrompts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Functions",
                table: "AIPrompts");
        }
    }
}
