using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class recurringmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomType",
                table: "L1RecurringPresets",
                newName: "IsThreshold");

            migrationBuilder.AlterColumn<int>(
                name: "RepeatingThreshold",
                table: "L1RecurringPresets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DaytoGenerate",
                table: "L1RecurringPresets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaytoGenerate",
                table: "L1RecurringPresets");

            migrationBuilder.RenameColumn(
                name: "IsThreshold",
                table: "L1RecurringPresets",
                newName: "CustomType");

            migrationBuilder.AlterColumn<int>(
                name: "RepeatingThreshold",
                table: "L1RecurringPresets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
