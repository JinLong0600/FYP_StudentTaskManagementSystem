using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class renamerecurringcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsThreshold",
                table: "L1RecurringPresets");

            migrationBuilder.RenameColumn(
                name: "RepeatingThreshold",
                table: "L1RecurringPresets",
                newName: "RecurringCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecurringCount",
                table: "L1RecurringPresets",
                newName: "RepeatingThreshold");

            migrationBuilder.AddColumn<int>(
                name: "IsThreshold",
                table: "L1RecurringPresets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
