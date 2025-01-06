using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class updateforum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "L1DiscussionId",
                table: "L1DiscussionForumComments",
                newName: "L1DiscussionForumId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionDateTime",
                table: "L1DiscussionForums",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByStudentId",
                table: "L1DiscussionForums",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DiscussionForumId",
                table: "L1DiscussionForumComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_L1DiscussionForumComments_L1DiscussionForumId",
                table: "L1DiscussionForumComments",
                column: "L1DiscussionForumId");

            migrationBuilder.AddForeignKey(
                name: "FK_L1DiscussionForumComments_L1DiscussionForums_L1DiscussionForumId",
                table: "L1DiscussionForumComments",
                column: "L1DiscussionForumId",
                principalTable: "L1DiscussionForums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_L1DiscussionForumComments_L1DiscussionForums_L1DiscussionForumId",
                table: "L1DiscussionForumComments");

            migrationBuilder.DropIndex(
                name: "IX_L1DiscussionForumComments_L1DiscussionForumId",
                table: "L1DiscussionForumComments");

            migrationBuilder.DropColumn(
                name: "DiscussionForumId",
                table: "L1DiscussionForumComments");

            migrationBuilder.RenameColumn(
                name: "L1DiscussionForumId",
                table: "L1DiscussionForumComments",
                newName: "L1DiscussionId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletionDateTime",
                table: "L1DiscussionForums",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByStudentId",
                table: "L1DiscussionForums",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
