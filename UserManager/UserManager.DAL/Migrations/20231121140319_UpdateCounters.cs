using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalTokensUsage",
                table: "Users",
                newName: "TotalPurchasedPoints");

            migrationBuilder.RenameColumn(
                name: "TotalPurchasedTokens",
                table: "Users",
                newName: "TotalPointsUsage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPurchasedPoints",
                table: "Users",
                newName: "TotalTokensUsage");

            migrationBuilder.RenameColumn(
                name: "TotalPointsUsage",
                table: "Users",
                newName: "TotalPurchasedTokens");
        }
    }
}
