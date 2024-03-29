using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ActionHistories_TicketId",
                table: "ActionHistories",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionHistories_Tickets_TicketId",
                table: "ActionHistories",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionHistories_Tickets_TicketId",
                table: "ActionHistories");

            migrationBuilder.DropIndex(
                name: "IX_ActionHistories_TicketId",
                table: "ActionHistories");
        }
    }
}
