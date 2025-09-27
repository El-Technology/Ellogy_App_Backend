using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations;

/// <inheritdoc />
public partial class AddTraceabilityMatrix : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserStoryTestTicketSummaries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserStoryTestId = table.Column<Guid>(type: "uuid", nullable: false),
                TicketSummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserStoryTestTicketSummaries", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserStoryTestTicketSummaries_TicketSummaries_TicketSummaryId",
                    column: x => x.TicketSummaryId,
                    principalTable: "TicketSummaries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserStoryTestTicketSummaries_UserStoryTest_UserStoryTestId",
                    column: x => x.UserStoryTestId,
                    principalTable: "UserStoryTest",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserStoryTestTicketSummaries_TicketSummaryId",
            table: "UserStoryTestTicketSummaries",
            column: "TicketSummaryId");

        migrationBuilder.CreateIndex(
            name: "IX_UserStoryTestTicketSummaries_UserStoryTestId_TicketSummaryId",
            table: "UserStoryTestTicketSummaries",
            columns: new[] { "UserStoryTestId", "TicketSummaryId" },
            unique: true);

        migrationBuilder.Sql(@"
            INSERT INTO \"UserStoryTestTicketSummaries\" (\"Id\", \"UserStoryTestId\", \"TicketSummaryId\", \"CreatedAt\")
            SELECT DISTINCT
                gen_random_uuid(),
                ust.\"Id\"      AS \"UserStoryTestId\",
                tsu.\"TicketSummaryId\"
            FROM \"UserStoryTest\" ust
            JOIN \"Usecases\" u ON u.\"Id\" = ust.\"UsecaseId\"
            JOIN \"TicketSummaryUsecase\" tsu ON tsu.\"UsecasesId\" = u.\"Id\"
            LEFT JOIN \"UserStoryTestTicketSummaries\" existing
                ON existing.\"UserStoryTestId\" = ust.\"Id\"
               AND existing.\"TicketSummaryId\" = tsu.\"TicketSummaryId\"
            WHERE existing.\"Id\" IS NULL;
        ");

        migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS \"vw_TraceabilityMatrix\";

            CREATE VIEW \"vw_TraceabilityMatrix\" AS
            SELECT
                ts.\"TicketId\",
                ts.\"Id\"              AS \"UserStoryId\",
                ts.\"Data\"            AS \"UserStory\",
                ts.\"SubStage\",
                ss.\"Id\"              AS \"ScenarioId\",
                ss.\"Title\"           AS \"ScenarioTitle\",
                ss.\"Description\"     AS \"ScenarioDescription\",
                sac.\"Id\"             AS \"AcceptanceId\",
                sac.\"Title\"          AS \"AcceptanceTitle\",
                sac.\"Description\"    AS \"AcceptanceDescription\",
                ust.\"Id\"             AS \"UserStoryTestId\",
                ust.\"Order\"          AS \"TestOrder\",
                ust.\"UsecaseId\",
                tc.\"Id\"              AS \"TestCaseId\",
                tc.\"TestCaseId\"      AS \"TestCaseRef\",
                tc.\"Description\"     AS \"TestCaseDescription\",
                tc.\"PreConditions\",
                tc.\"TestSteps\",
                tc.\"TestData\",
                tc.\"ExpectedResult\"
            FROM \"TicketSummaries\" ts
            LEFT JOIN \"SummaryScenario\" ss
                   ON ss.\"TicketSummaryId\" = ts.\"Id\"
            LEFT JOIN \"SummaryAcceptanceCriteria\" sac
                   ON sac.\"TicketSummaryId\" = ts.\"Id\"
            LEFT JOIN \"UserStoryTestTicketSummaries\" ustts
                   ON ustts.\"TicketSummaryId\" = ts.\"Id\"
            LEFT JOIN \"UserStoryTest\" ust
                   ON ust.\"Id\" = ustts.\"UserStoryTestId\"
            LEFT JOIN \"TestCase\" tc
                   ON tc.\"UserStoryTestId\" = ust.\"Id\";
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP VIEW IF EXISTS \"vw_TraceabilityMatrix\";");

        migrationBuilder.DropTable(
            name: "UserStoryTestTicketSummaries");
    }
}
