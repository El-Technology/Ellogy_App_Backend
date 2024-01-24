using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeCustomerIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Users");
        }
    }
}
