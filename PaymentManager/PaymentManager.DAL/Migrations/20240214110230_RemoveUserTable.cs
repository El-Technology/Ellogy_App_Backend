using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
