using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSentToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeSent",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSent",
                table: "Messages");
        }
    }
}
