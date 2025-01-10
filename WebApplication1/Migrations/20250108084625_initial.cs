using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "L1RecurringPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DaytoGenerate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecurringCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RecurringPresets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1RecurringTaskCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    L1TaskId = table.Column<int>(type: "int", nullable: false),
                    L1RecurringTaskSettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1RecurringTaskCounters", x => x.Id);
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
                    GuardianRelationship = table.Column<int>(type: "int", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "L1DiscussionForums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionForums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1DiscussionForums_L1Students_CreatedByStudentId",
                        column: x => x.CreatedByStudentId,
                        principalTable: "L1Students",
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
                    ReminderTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDaily = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "L1DiscussionForumComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1DiscussionForumId = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionForumComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1DiscussionForumComments_L1DiscussionForums_L1DiscussionForumId",
                        column: x => x.L1DiscussionForumId,
                        principalTable: "L1DiscussionForums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_L1DiscussionForumComments_L1Students_CreatedByStudentId",
                        column: x => x.CreatedByStudentId,
                        principalTable: "L1Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "L1DiscussionForumLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1DiscussionForumId = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionForumLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1DiscussionForumLikes_L1DiscussionForums_L1DiscussionForumId",
                        column: x => x.L1DiscussionForumId,
                        principalTable: "L1DiscussionForums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_L1DiscussionForumLikes_L1Students_CreatedByStudentId",
                        column: x => x.CreatedByStudentId,
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
                    Priority = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    IsParentRecurring = table.Column<bool>(type: "bit", nullable: true),
                    L1RecurringPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultRecurringOptions = table.Column<int>(type: "int", nullable: true),
                    GeneratedCount = table.Column<int>(type: "int", nullable: true),
                    IsNotification = table.Column<bool>(type: "bit", nullable: false),
                    L1NotificationPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultNotificationOptions = table.Column<int>(type: "int", nullable: true),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_L1Tasks_L1NotificationPresets_L1NotificationPresetId",
                        column: x => x.L1NotificationPresetId,
                        principalTable: "L1NotificationPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_L1Tasks_L1RecurringPresets_L1RecurringPresetId",
                        column: x => x.L1RecurringPresetId,
                        principalTable: "L1RecurringPresets",
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
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsNotification = table.Column<bool>(type: "bit", nullable: false),
                    L1NotificationPresetId = table.Column<int>(type: "int", nullable: true),
                    DefaultNotificationOptions = table.Column<int>(type: "int", nullable: true),
                    CreatedByStudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    L1NotificationPresetsId = table.Column<int>(type: "int", nullable: true),
                    L1RecurringPatternsId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_L1SubTasks_L1RecurringPresets_L1RecurringPatternsId",
                        column: x => x.L1RecurringPatternsId,
                        principalTable: "L1RecurringPresets",
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
                name: "IX_L1DiscussionForumComments_CreatedByStudentId",
                table: "L1DiscussionForumComments",
                column: "CreatedByStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_L1DiscussionForumComments_L1DiscussionForumId",
                table: "L1DiscussionForumComments",
                column: "L1DiscussionForumId");

            migrationBuilder.CreateIndex(
                name: "IX_L1DiscussionForumLikes_CreatedByStudentId",
                table: "L1DiscussionForumLikes",
                column: "CreatedByStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_L1DiscussionForumLikes_L1DiscussionForumId",
                table: "L1DiscussionForumLikes",
                column: "L1DiscussionForumId");

            migrationBuilder.CreateIndex(
                name: "IX_L1DiscussionForums_CreatedByStudentId",
                table: "L1DiscussionForums",
                column: "CreatedByStudentId");

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
                name: "IX_L1SubTasks_L1RecurringPatternsId",
                table: "L1SubTasks",
                column: "L1RecurringPatternsId");

            migrationBuilder.CreateIndex(
                name: "IX_L1SubTasks_L1TaskId",
                table: "L1SubTasks",
                column: "L1TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_L1Tasks_L1NotificationPresetId",
                table: "L1Tasks",
                column: "L1NotificationPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_L1Tasks_L1RecurringPresetId",
                table: "L1Tasks",
                column: "L1RecurringPresetId");

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
                name: "L1DiscussionForumComments");

            migrationBuilder.DropTable(
                name: "L1DiscussionForumLikes");

            migrationBuilder.DropTable(
                name: "L1RecurringTaskCounters");

            migrationBuilder.DropTable(
                name: "L1RoleClaims");

            migrationBuilder.DropTable(
                name: "L1SubTasks");

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
                name: "L1DiscussionForums");

            migrationBuilder.DropTable(
                name: "L1Tasks");

            migrationBuilder.DropTable(
                name: "L1Roles");

            migrationBuilder.DropTable(
                name: "L1NotificationPresets");

            migrationBuilder.DropTable(
                name: "L1RecurringPresets");

            migrationBuilder.DropTable(
                name: "L1Students");
        }
    }
}
