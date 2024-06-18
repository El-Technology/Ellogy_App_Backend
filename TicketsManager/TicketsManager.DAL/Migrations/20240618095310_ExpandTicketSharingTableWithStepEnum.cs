using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ExpandTicketSharingTableWithStepEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubStageEnum",
                table: "TicketShares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCurrentStep",
                table: "TicketShares",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubStageEnum",
                table: "TicketShares");

            migrationBuilder.DropColumn(
                name: "TicketCurrentStep",
                table: "TicketShares");
        }
    }
}
