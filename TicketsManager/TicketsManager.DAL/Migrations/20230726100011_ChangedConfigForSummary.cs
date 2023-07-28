using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedConfigForSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSummaries_Tickets_Id",
                table: "TicketSummaries");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSummaries_TicketId",
                table: "TicketSummaries",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSummaries_Tickets_TicketId",
                table: "TicketSummaries",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSummaries_Tickets_TicketId",
                table: "TicketSummaries");

            migrationBuilder.DropIndex(
                name: "IX_TicketSummaries_TicketId",
                table: "TicketSummaries");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSummaries_Tickets_Id",
                table: "TicketSummaries",
                column: "Id",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
