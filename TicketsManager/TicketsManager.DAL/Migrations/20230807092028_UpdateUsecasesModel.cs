using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsecasesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketDiagrams_Tickets_TicketId",
                table: "TicketDiagrams");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTables_Tickets_TicketId",
                table: "TicketTables");

            migrationBuilder.DropTable(
                name: "TicketTableValues");

            migrationBuilder.DropIndex(
                name: "IX_TicketTables_TicketId",
                table: "TicketTables");

            migrationBuilder.DropIndex(
                name: "IX_TicketDiagrams_TicketId",
                table: "TicketDiagrams");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "TicketTables",
                newName: "UsecaseId");

            migrationBuilder.RenameColumn(
                name: "TableKey",
                table: "TicketTables",
                newName: "Table");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "TicketDiagrams",
                newName: "UsecaseId");

            migrationBuilder.CreateTable(
                name: "Usecases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usecases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usecases_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Usecases_TicketId",
                table: "Usecases",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketDiagrams_Usecases_UsecaseId",
                table: "TicketDiagrams",
                column: "UsecaseId",
                principalTable: "Usecases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTables_Usecases_UsecaseId",
                table: "TicketTables",
                column: "UsecaseId",
                principalTable: "Usecases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketDiagrams_Usecases_UsecaseId",
                table: "TicketDiagrams");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTables_Usecases_UsecaseId",
                table: "TicketTables");

            migrationBuilder.DropTable(
                name: "Usecases");

            migrationBuilder.DropIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables");

            migrationBuilder.DropIndex(
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams");

            migrationBuilder.RenameColumn(
                name: "UsecaseId",
                table: "TicketTables",
                newName: "TicketId");

            migrationBuilder.RenameColumn(
                name: "Table",
                table: "TicketTables",
                newName: "TableKey");

            migrationBuilder.RenameColumn(
                name: "UsecaseId",
                table: "TicketDiagrams",
                newName: "TicketId");

            migrationBuilder.CreateTable(
                name: "TicketTableValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTableValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTableValues_TicketTables_TicketTableId",
                        column: x => x.TicketTableId,
                        principalTable: "TicketTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTables_TicketId",
                table: "TicketTables",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDiagrams_TicketId",
                table: "TicketDiagrams",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTableValues_TicketTableId",
                table: "TicketTableValues",
                column: "TicketTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketDiagrams_Tickets_TicketId",
                table: "TicketDiagrams",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTables_Tickets_TicketId",
                table: "TicketTables",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
