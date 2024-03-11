using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUsecaseRalationToUserStoryTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsecaseId",
                table: "UserStoryTest",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryTest_UsecaseId",
                table: "UserStoryTest",
                column: "UsecaseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest",
                column: "UsecaseId",
                principalTable: "Usecases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStoryTest_Usecases_UsecaseId",
                table: "UserStoryTest");

            migrationBuilder.DropIndex(
                name: "IX_UserStoryTest_UsecaseId",
                table: "UserStoryTest");

            migrationBuilder.DropColumn(
                name: "UsecaseId",
                table: "UserStoryTest");
        }
    }
}
