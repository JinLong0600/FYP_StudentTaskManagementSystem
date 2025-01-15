using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class NQupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1NotificationPresets_NotificationPresetId",
                table: "NotificationQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1Tasks_TaskId",
                table: "NotificationQueues");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_NotificationPresetId",
                table: "NotificationQueues");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_TaskId",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "NotificationPresetId",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "NotificationQueues");

            migrationBuilder.AlterColumn<int>(
                name: "L1TaskId",
                table: "NotificationQueues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_L1NotificationPresetId",
                table: "NotificationQueues",
                column: "L1NotificationPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_L1TaskId",
                table: "NotificationQueues",
                column: "L1TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1NotificationPresets_L1NotificationPresetId",
                table: "NotificationQueues",
                column: "L1NotificationPresetId",
                principalTable: "L1NotificationPresets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1Tasks_L1TaskId",
                table: "NotificationQueues",
                column: "L1TaskId",
                principalTable: "L1Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1NotificationPresets_L1NotificationPresetId",
                table: "NotificationQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1Tasks_L1TaskId",
                table: "NotificationQueues");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_L1NotificationPresetId",
                table: "NotificationQueues");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_L1TaskId",
                table: "NotificationQueues");

            migrationBuilder.AlterColumn<int>(
                name: "L1TaskId",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationPresetId",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_NotificationPresetId",
                table: "NotificationQueues",
                column: "NotificationPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_TaskId",
                table: "NotificationQueues",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1NotificationPresets_NotificationPresetId",
                table: "NotificationQueues",
                column: "NotificationPresetId",
                principalTable: "L1NotificationPresets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1Tasks_TaskId",
                table: "NotificationQueues",
                column: "TaskId",
                principalTable: "L1Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
