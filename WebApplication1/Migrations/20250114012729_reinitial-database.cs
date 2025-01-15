using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class reinitialdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1SubTasks_SubtaskId",
                table: "NotificationQueues");

            migrationBuilder.DropTable(
                name: "L1TaskReminders");

            migrationBuilder.DropColumn(
                name: "GroupKey",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "IsDaily",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "TaskDueDate",
                table: "NotificationQueues");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PushSubscriptions",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "NotificationQueues",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "SubtaskId",
                table: "NotificationQueues",
                newName: "L1SubTasksId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "NotificationQueues",
                newName: "L1TaskId");

            migrationBuilder.RenameColumn(
                name: "NotificationType",
                table: "NotificationQueues",
                newName: "L1NotificationPresetId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationQueues_SubtaskId",
                table: "NotificationQueues",
                newName: "IX_NotificationQueues_L1SubTasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1SubTasks_L1SubTasksId",
                table: "NotificationQueues",
                column: "L1SubTasksId",
                principalTable: "L1SubTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_L1SubTasks_L1SubTasksId",
                table: "NotificationQueues");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "PushSubscriptions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "NotificationQueues",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "L1TaskId",
                table: "NotificationQueues",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "L1SubTasksId",
                table: "NotificationQueues",
                newName: "SubtaskId");

            migrationBuilder.RenameColumn(
                name: "L1NotificationPresetId",
                table: "NotificationQueues",
                newName: "NotificationType");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationQueues_L1SubTasksId",
                table: "NotificationQueues",
                newName: "IX_NotificationQueues_SubtaskId");

            migrationBuilder.AddColumn<string>(
                name: "GroupKey",
                table: "NotificationQueues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDaily",
                table: "NotificationQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskDueDate",
                table: "NotificationQueues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "L1TaskReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsTriggered = table.Column<bool>(type: "bit", nullable: false),
                    L1TaskId = table.Column<int>(type: "int", nullable: false),
                    ReminderDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1TaskReminders", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_L1SubTasks_SubtaskId",
                table: "NotificationQueues",
                column: "SubtaskId",
                principalTable: "L1SubTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
