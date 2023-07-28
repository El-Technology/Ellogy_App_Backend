using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamesForActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionTypeEnum",
                table: "Messages",
                newName: "ActionType");

            migrationBuilder.RenameColumn(
                name: "ActionStateEnum",
                table: "Messages",
                newName: "ActionState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "Messages",
                newName: "ActionTypeEnum");

            migrationBuilder.RenameColumn(
                name: "ActionState",
                table: "Messages",
                newName: "ActionStateEnum");
        }
    }
}
