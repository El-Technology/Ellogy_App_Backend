using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalTokensCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPurchasedTokens",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalTokensUsage",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPurchasedTokens",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalTokensUsage",
                table: "Users");
        }
    }
}
