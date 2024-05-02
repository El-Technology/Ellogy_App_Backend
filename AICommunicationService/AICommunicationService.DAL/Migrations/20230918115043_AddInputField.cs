using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AICommunicationService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInputField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Input",
                table: "AIPrompts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Input",
                table: "AIPrompts");
        }
    }
}
