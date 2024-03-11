using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUsecaseRalationToTicketSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketSummaryUsecase",
                columns: table => new
                {
                    TicketSummariesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsecasesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSummaryUsecase", x => new { x.TicketSummariesId, x.UsecasesId });
                    table.ForeignKey(
                        name: "FK_TicketSummaryUsecase_TicketSummaries_TicketSummariesId",
                        column: x => x.TicketSummariesId,
                        principalTable: "TicketSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketSummaryUsecase_Usecases_UsecasesId",
                        column: x => x.UsecasesId,
                        principalTable: "Usecases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSummaryUsecase_UsecasesId",
                table: "TicketSummaryUsecase",
                column: "UsecasesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketSummaryUsecase");
        }
    }
}
