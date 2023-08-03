using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AICommunicationService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AiPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIPrompts",
                columns: table => new
                {
                    TamplateName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIPrompts", x => x.TamplateName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIPrompts");
        }
    }
}
