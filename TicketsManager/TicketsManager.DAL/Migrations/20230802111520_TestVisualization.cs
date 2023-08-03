using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TestVisualization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "Messages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketDiagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PictureLink = table.Column<string>(type: "text", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketDiagrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketDiagrams_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TableKey = table.Column<string>(type: "text", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTables_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketTableValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    TicketTableId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "IX_TicketDiagrams_TicketId",
                table: "TicketDiagrams",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTables_TicketId",
                table: "TicketTables",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTableValues_TicketTableId",
                table: "TicketTableValues",
                column: "TicketTableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketDiagrams");

            migrationBuilder.DropTable(
                name: "TicketTableValues");

            migrationBuilder.DropTable(
                name: "TicketTables");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Messages");
        }
    }
}
