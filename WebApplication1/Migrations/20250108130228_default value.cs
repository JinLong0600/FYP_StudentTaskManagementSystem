using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class defaultvalue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefault",
                table: "L1RecurringPresets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefault",
                table: "L1NotificationPresets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemDefault",
                table: "L1RecurringPresets");

            migrationBuilder.DropColumn(
                name: "IsSystemDefault",
                table: "L1NotificationPresets");
        }
    }
}
