using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class L1DiscussionForumComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L1DiscussionComments");

            migrationBuilder.DropTable(
                name: "L1Discussions");

            migrationBuilder.CreateTable(
                name: "L1DiscussionForumComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1DiscussionId = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionForumComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1DiscussionForums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1DiscussionForums", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L1DiscussionForumComments");

            migrationBuilder.DropTable(
                name: "L1DiscussionForums");

            migrationBuilder.CreateTable(
                name: "L1DiscussionComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTakenDown = table.Column<bool>(type: "bit", nullable: false),
                    L0AdminId = table.Column<int>(type: "int", nullable: false),
                    L1DiscussionId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    CreatedByStudentId = table.Column<int>(type: "int", nullable: false),
                    DeletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InsitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTakenDown = table.Column<bool>(type: "bit", nullable: false),
                    L0AdminId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrivacySetting = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Discussions", x => x.Id);
                });
        }
    }
}
