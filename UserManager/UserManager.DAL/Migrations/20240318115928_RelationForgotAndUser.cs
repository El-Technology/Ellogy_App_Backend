using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RelationForgotAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ForgotPassword_UserId",
                table: "ForgotPassword",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForgotPassword_Users_UserId",
                table: "ForgotPassword",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForgotPassword_Users_UserId",
                table: "ForgotPassword");

            migrationBuilder.DropIndex(
                name: "IX_ForgotPassword_UserId",
                table: "ForgotPassword");
        }
    }
}
