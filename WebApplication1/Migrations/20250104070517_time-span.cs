using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class timespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L1DiscussionCommentLikeCounts");

            migrationBuilder.DropTable(
                name: "L1ReportDiscussionSubmissions");

            migrationBuilder.DropTable(
                name: "L1SuspendedHistories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReminderTime",
                table: "L1NotificationPresets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ReminderTime",
                table: "L1NotificationPresets",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "L1DiscussionCommentLikeCounts",
                columns: table => new
                {
                    L1DiscussionCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionCommentLikeCounts", x => x.L1DiscussionCommentId);
                });

            migrationBuilder.CreateTable(
                name: "L1ReportDiscussionSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminReply = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    L1DiscussionCommentId = table.Column<int>(type: "int", nullable: false),
                    L1DiscussionId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedByAdminId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1ReportDiscussionSubmissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1SuspendedHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsParentTask = table.Column<bool>(type: "bit", nullable: false),
                    IsRecurringTask = table.Column<bool>(type: "bit", nullable: false),
                    IsReminderNeeded = table.Column<bool>(type: "bit", nullable: false),
                    L1ReminderSettingId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1SuspendedHistories", x => x.Id);
                });
        }
    }
}
