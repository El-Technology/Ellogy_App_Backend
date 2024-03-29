using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionHistoryEnum = table.Column<int>(type: "integer", nullable: false),
                    TicketCurrentStepEnum = table.Column<int>(type: "integer", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: true),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ActionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Context = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    BannersJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: true),
                    ActionState = table.Column<int>(type: "integer", nullable: true),
                    Stage = table.Column<int>(type: "integer", nullable: true),
                    SubStage = table.Column<int>(type: "integer", nullable: true),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Sms = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<bool>(type: "boolean", nullable: false),
                    Push = table.Column<bool>(type: "boolean", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    IsPotential = table.Column<bool>(type: "boolean", nullable: false),
                    SubStage = table.Column<int>(type: "integer", nullable: true),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSummaries_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usecases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usecases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usecases_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryAcceptanceCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TicketSummaryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryAcceptanceCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryAcceptanceCriteria_TicketSummaries_TicketSummaryId",
                        column: x => x.TicketSummaryId,
                        principalTable: "TicketSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryScenario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TicketSummaryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryScenario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryScenario_TicketSummaries_TicketSummaryId",
                        column: x => x.TicketSummaryId,
                        principalTable: "TicketSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketDiagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PictureLink = table.Column<string>(type: "text", nullable: false),
                    UsecaseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketDiagrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketDiagrams_Usecases_UsecaseId",
                        column: x => x.UsecaseId,
                        principalTable: "Usecases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSummaryUsecase",
                columns: table => new
                {
                    TicketSummariesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsecasesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSummaryUsecase", x => new { x.TicketSummariesId, x.UsecasesId });
                    table.ForeignKey(
                        name: "FK_TicketSummaryUsecase_TicketSummaries_TicketSummariesId",
                        column: x => x.TicketSummariesId,
                        principalTable: "TicketSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketSummaryUsecase_Usecases_UsecasesId",
                        column: x => x.UsecasesId,
                        principalTable: "Usecases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Table = table.Column<string>(type: "text", nullable: false),
                    UsecaseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTables_Usecases_UsecaseId",
                        column: x => x.UsecaseId,
                        principalTable: "Usecases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStoryTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestScenarios = table.Column<string>(type: "text", nullable: false),
                    UsecaseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoryTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStoryTest_Usecases_UsecaseId",
                        column: x => x.UsecaseId,
                        principalTable: "Usecases",
                        principalColumn: "Id");
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
                name: "IX_Messages_TicketId",
                table: "Messages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TicketId",
                table: "Notifications",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryAcceptanceCriteria_TicketSummaryId",
                table: "SummaryAcceptanceCriteria",
                column: "TicketSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryScenario_TicketSummaryId",
                table: "SummaryScenario",
                column: "TicketSummaryId");

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
                name: "IX_TicketDiagrams_UsecaseId",
                table: "TicketDiagrams",
                column: "UsecaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSummaries_TicketId",
                table: "TicketSummaries",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSummaryUsecase_UsecasesId",
                table: "TicketSummaryUsecase",
                column: "UsecasesId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTables_UsecaseId",
                table: "TicketTables",
                column: "UsecaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Usecases_TicketId",
                table: "Usecases",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryTest_UsecaseId",
                table: "UserStoryTest",
                column: "UsecaseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionHistories");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SummaryAcceptanceCriteria");

            migrationBuilder.DropTable(
                name: "SummaryScenario");

            migrationBuilder.DropTable(
                name: "TestCase");

            migrationBuilder.DropTable(
                name: "TestPlan");

            migrationBuilder.DropTable(
                name: "TicketDiagrams");

            migrationBuilder.DropTable(
                name: "TicketSummaryUsecase");

            migrationBuilder.DropTable(
                name: "TicketTables");

            migrationBuilder.DropTable(
                name: "UserStoryTest");

            migrationBuilder.DropTable(
                name: "TicketSummaries");

            migrationBuilder.DropTable(
                name: "Usecases");

            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
