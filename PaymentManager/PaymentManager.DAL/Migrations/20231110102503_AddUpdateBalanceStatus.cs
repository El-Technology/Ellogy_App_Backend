using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateBalanceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UpdatedBallance",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBallance",
                table: "Payments");
        }
    }
}
