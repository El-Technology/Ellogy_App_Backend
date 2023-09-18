using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AICommunicationService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TamplateName",
                table: "AIPrompts",
                newName: "TemplateName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateName",
                table: "AIPrompts",
                newName: "TamplateName");
        }
    }
}
