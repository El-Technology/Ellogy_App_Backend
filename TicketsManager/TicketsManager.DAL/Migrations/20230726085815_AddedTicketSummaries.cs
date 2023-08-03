using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedTicketSummaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "TicketSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    IsPotential = table.Column<bool>(type: "boolean", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSummaries_Tickets_Id",
                        column: x => x.Id,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketSummaries");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Tickets",
                type: "text",
                nullable: true);
        }
    }
}
