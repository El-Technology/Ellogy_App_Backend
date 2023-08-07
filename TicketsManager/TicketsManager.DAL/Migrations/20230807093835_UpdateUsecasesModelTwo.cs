using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsecasesModelTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables");

            migrationBuilder.DropIndex(
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables",
                column: "UsecaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams",
                column: "UsecaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables");

            migrationBuilder.DropIndex(
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables",
                column: "UsecaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams",
                column: "UsecaseId",
                unique: true);
        }
    }
}
