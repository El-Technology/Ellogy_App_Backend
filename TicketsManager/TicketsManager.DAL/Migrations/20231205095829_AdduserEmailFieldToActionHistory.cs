using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdduserEmailFieldToActionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "ActionHistories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "ActionHistories");
        }
    }
}
