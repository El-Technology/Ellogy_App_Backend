using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserStoryTestField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStoryTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestScenarios = table.Column<string>(type: "text", nullable: false),
                    TicketSummaryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoryTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStoryTest_TicketSummaries_TicketSummaryId",
                        column: x => x.TicketSummaryId,
                        principalTable: "TicketSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestCaseId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PreConditions = table.Column<string>(type: "text", nullable: false),
                    TestSteps = table.Column<string>(type: "text", nullable: false),
                    TestData = table.Column<string>(type: "text", nullable: false),
                    ExpectedResult = table.Column<string>(type: "text", nullable: false),
                    UserStoryTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCase_UserStoryTest_UserStoryTestId",
                        column: x => x.UserStoryTestId,
                        principalTable: "UserStoryTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Objective = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    Resources = table.Column<string>(type: "text", nullable: false),
                    Schedule = table.Column<string>(type: "text", nullable: false),
                    TestEnvironment = table.Column<string>(type: "text", nullable: false),
                    RiskManagement = table.Column<string>(type: "text", nullable: false),
                    UserStoryTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestPlan_UserStoryTest_UserStoryTestId",
                        column: x => x.UserStoryTestId,
                        principalTable: "UserStoryTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCase_UserStoryTestId",
                table: "TestCase",
                column: "UserStoryTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_UserStoryTestId",
                table: "TestPlan",
                column: "UserStoryTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryTest_TicketSummaryId",
                table: "UserStoryTest",
                column: "TicketSummaryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCase");

            migrationBuilder.DropTable(
                name: "TestPlan");

            migrationBuilder.DropTable(
                name: "UserStoryTest");
        }
    }
}
