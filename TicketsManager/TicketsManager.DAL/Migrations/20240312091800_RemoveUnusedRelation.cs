using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStoryTest_TicketSummaries_TicketSummaryId",
                table: "UserStoryTest");

            migrationBuilder.DropIndex(
                name: "IX_UserStoryTest_TicketSummaryId",
                table: "UserStoryTest");

            migrationBuilder.DropColumn(
                name: "TicketSummaryId",
                table: "UserStoryTest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketSummaryId",
                table: "UserStoryTest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryTest_TicketSummaryId",
                table: "UserStoryTest",
                column: "TicketSummaryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStoryTest_TicketSummaries_TicketSummaryId",
                table: "UserStoryTest",
                column: "TicketSummaryId",
                principalTable: "TicketSummaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
