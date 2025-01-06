using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "L0Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProfileImage = table.Column<int>(type: "int", maxLength: 500, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    CountryAccess = table.Column<int>(type: "int", nullable: false),
                    CountryCode = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    CityCode = table.Column<int>(type: "int", maxLength: 3, nullable: false),
                    CreatedByAdminId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedByAdminId = table.Column<int>(type: "int", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L0Admins", x => x.Id);
                });

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
                name: "L1DiscussionComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1DiscussionId = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTakenDown = table.Column<bool>(type: "bit", nullable: false),
                    L0AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1Discussions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InsitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrivacySetting = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTakenDown = table.Column<bool>(type: "bit", nullable: false),
                    L0AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Discussions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1RecurringTaskCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L1TaskId = table.Column<int>(type: "int", nullable: false),
                    L1RecurringTaskSettingId = table.Column<int>(type: "int", nullable: false),
                    Counter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RecurringTaskCounters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1RecurringTaskSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDays = table.Column<int>(type: "int", nullable: false),
                    RepeatingCount = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RecurringTaskSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1ReportDiscussionSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L1DiscussionId = table.Column<int>(type: "int", nullable: false),
                    L1DiscussionCommentId = table.Column<int>(type: "int", nullable: false),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    ReportReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    SubmissionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedByAdminId = table.Column<int>(type: "int", nullable: false),
                    AdminReply = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1ReportDiscussionSubmissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    GuardianRelationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentIdentityCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationStage = table.Column<int>(type: "int", nullable: false),
                    EducationalYear = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<int>(type: "int", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAccountDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SuspensionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedByAdminId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1SuspendedHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    IsRecurringTask = table.Column<bool>(type: "bit", nullable: false),
                    IsParentTask = table.Column<bool>(type: "bit", nullable: false),
                    IsReminderNeeded = table.Column<bool>(type: "bit", nullable: false),
                    L1ReminderSettingId = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1SuspendedHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1TaskReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1TaskId = table.Column<int>(type: "int", nullable: false),
                    ReminderDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTriggered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1TaskReminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1RoleClaims_L1Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "L1Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1NotificationPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReminderDaysBefore = table.Column<int>(type: "int", nullable: true),
                    ReminderHoursBefore = table.Column<int>(type: "int", nullable: true),
                    ReminderMinutesBefore = table.Column<int>(type: "int", nullable: true),
                    ReminderTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsDaily = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1NotificationPresets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1NotificationPresets_L1Students_UserId",
                        column: x => x.UserId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1UserClaims_L1Students_UserId",
                        column: x => x.UserId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_L1UserLogins_L1Students_UserId",
                        column: x => x.UserId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_L1UserRoles_L1Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "L1Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_L1UserRoles_L1Students_UserId",
                        column: x => x.UserId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_L1UserTokens_L1Students_UserId",
                        column: x => x.UserId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    L1RecurringPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultRecurringOptions = table.Column<int>(type: "int", nullable: true),
                    IsNotification = table.Column<bool>(type: "bit", nullable: false),
                    L1NotificationPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultNotificationOptions = table.Column<int>(type: "int", nullable: true),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    L1NotificationPresetsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1Tasks_L1NotificationPresets_L1NotificationPresetsId",
                        column: x => x.L1NotificationPresetsId,
                        principalTable: "L1NotificationPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1SubTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1TaskId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsNotification = table.Column<bool>(type: "bit", nullable: false),
                    L1NotificationPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultNotificationOptions = table.Column<int>(type: "int", nullable: true),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    L1NotificationPresetsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1SubTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1SubTasks_L1NotificationPresets_L1NotificationPresetsId",
                        column: x => x.L1NotificationPresetsId,
                        principalTable: "L1NotificationPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_L1SubTasks_L1Tasks_L1TaskId",
                        column: x => x.L1TaskId,
                        principalTable: "L1Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_L1NotificationPresets_UserId",
                table: "L1NotificationPresets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_L1RoleClaims_RoleId",
                table: "L1RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "L1Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "L1Students",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "L1Students",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_L1SubTasks_L1NotificationPresetsId",
                table: "L1SubTasks",
                column: "L1NotificationPresetsId");

            migrationBuilder.CreateIndex(
                name: "IX_L1SubTasks_L1TaskId",
                table: "L1SubTasks",
                column: "L1TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_L1Tasks_L1NotificationPresetsId",
                table: "L1Tasks",
                column: "L1NotificationPresetsId");

            migrationBuilder.CreateIndex(
                name: "IX_L1UserClaims_UserId",
                table: "L1UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_L1UserLogins_UserId",
                table: "L1UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_L1UserRoles_RoleId",
                table: "L1UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L0Admins");

            migrationBuilder.DropTable(
                name: "L1DiscussionCommentLikeCounts");

            migrationBuilder.DropTable(
                name: "L1DiscussionComments");

            migrationBuilder.DropTable(
                name: "L1Discussions");

            migrationBuilder.DropTable(
                name: "L1RecurringTaskCounters");

            migrationBuilder.DropTable(
                name: "L1RecurringTaskSettings");

            migrationBuilder.DropTable(
                name: "L1ReportDiscussionSubmissions");

            migrationBuilder.DropTable(
                name: "L1RoleClaims");

            migrationBuilder.DropTable(
                name: "L1SubTasks");

            migrationBuilder.DropTable(
                name: "L1SuspendedHistories");

            migrationBuilder.DropTable(
                name: "L1TaskReminders");

            migrationBuilder.DropTable(
                name: "L1UserClaims");

            migrationBuilder.DropTable(
                name: "L1UserLogins");

            migrationBuilder.DropTable(
                name: "L1UserRoles");

            migrationBuilder.DropTable(
                name: "L1UserTokens");

            migrationBuilder.DropTable(
                name: "L1Tasks");

            migrationBuilder.DropTable(
                name: "L1Roles");

            migrationBuilder.DropTable(
                name: "L1NotificationPresets");

            migrationBuilder.DropTable(
                name: "L1Students");
        }
    }
}
