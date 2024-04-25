using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UsecaseDeleteBehaviorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest",
                column: "UsecaseId",
                principalTable: "Usecases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest",
                column: "UsecaseId",
                principalTable: "Usecases",
                principalColumn: "Id");
        }
    }
}
