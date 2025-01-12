using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class adddb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_L1SubTasks_L1NotificationPresets_L1NotificationPresetsId",
                table: "L1SubTasks");

            migrationBuilder.DropTable(
                name: "L1RecurringTaskCounters");

            migrationBuilder.DropIndex(
                name: "IX_L1SubTasks_L1NotificationPresetsId",
                table: "L1SubTasks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "L1SubTasks");

            migrationBuilder.DropColumn(
                name: "L1NotificationPresetsId",
                table: "L1SubTasks");

            migrationBuilder.CreateTable(
                name: "NotificationQueues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SubtaskId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationPresetId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAttempt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GroupKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    IsDaily = table.Column<bool>(type: "bit", nullable: false),
                    TaskDueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationQueues_L1NotificationPresets_NotificationPresetId",
                        column: x => x.NotificationPresetId,
                        principalTable: "L1NotificationPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationQueues_L1SubTasks_SubtaskId",
                        column: x => x.SubtaskId,
                        principalTable: "L1SubTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationQueues_L1Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "L1Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PushSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    P256dh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Auth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_L1SubTasks_L1NotificationPresetId",
                table: "L1SubTasks",
                column: "L1NotificationPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_NotificationPresetId",
                table: "NotificationQueues",
                column: "NotificationPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_SubtaskId",
                table: "NotificationQueues",
                column: "SubtaskId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_TaskId",
                table: "NotificationQueues",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_L1SubTasks_L1NotificationPresets_L1NotificationPresetId",
                table: "L1SubTasks",
                column: "L1NotificationPresetId",
                principalTable: "L1NotificationPresets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_L1SubTasks_L1NotificationPresets_L1NotificationPresetId",
                table: "L1SubTasks");

            migrationBuilder.DropTable(
                name: "NotificationQueues");

            migrationBuilder.DropTable(
                name: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_L1SubTasks_L1NotificationPresetId",
                table: "L1SubTasks");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "L1SubTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "L1NotificationPresetsId",
                table: "L1SubTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "L1RecurringTaskCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L1RecurringTaskSettingId = table.Column<int>(type: "int", nullable: false),
                    L1TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RecurringTaskCounters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_L1SubTasks_L1NotificationPresetsId",
                table: "L1SubTasks",
                column: "L1NotificationPresetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_L1SubTasks_L1NotificationPresets_L1NotificationPresetsId",
                table: "L1SubTasks",
                column: "L1NotificationPresetsId",
                principalTable: "L1NotificationPresets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
