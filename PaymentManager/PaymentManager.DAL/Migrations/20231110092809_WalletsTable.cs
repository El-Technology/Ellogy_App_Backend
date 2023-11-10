using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class WalletsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Payments");
        }
    }
}
