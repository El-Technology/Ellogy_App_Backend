using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class subTicketEnumForMessageAndSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubStage",
                table: "TicketSummaries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubStage",
                table: "Messages",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubStage",
                table: "TicketSummaries");

            migrationBuilder.DropColumn(
                name: "SubStage",
                table: "Messages");
        }
    }
}
