using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations;

/// <inheritdoc />
public partial class AddUserIdFieldToMessageTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "Messages",
            type: "uuid",
            nullable: true);

        migrationBuilder.Sql(
            @"
            UPDATE ""Messages"" ms
            SET ""UserId"" = ts.""UserId""
            FROM ""Tickets"" ts
            WHERE ms.""TicketId"" = ts.""Id""
            "
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "Messages");
    }
}
