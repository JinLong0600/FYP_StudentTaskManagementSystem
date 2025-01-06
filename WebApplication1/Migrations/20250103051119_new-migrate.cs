using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class newmigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Counter",
                table: "L1RecurringTaskCounters");

            migrationBuilder.RenameColumn(
                name: "StartDays",
                table: "L1RecurringPresets",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RepeatingCount",
                table: "L1RecurringPresets",
                newName: "RepeatingThreshold");

            migrationBuilder.AddColumn<int>(
                name: "CustomType",
                table: "L1RecurringPresets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "L1NotificationPresets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomType",
                table: "L1RecurringPresets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "L1NotificationPresets");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "L1RecurringPresets",
                newName: "StartDays");

            migrationBuilder.RenameColumn(
                name: "RepeatingThreshold",
                table: "L1RecurringPresets",
                newName: "RepeatingCount");

            migrationBuilder.AddColumn<int>(
                name: "Counter",
                table: "L1RecurringTaskCounters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
