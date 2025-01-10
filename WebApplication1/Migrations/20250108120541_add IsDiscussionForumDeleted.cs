using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class addIsDiscussionForumDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscussionForumDeleted",
                table: "L1DiscussionForumComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscussionForumDeleted",
                table: "L1DiscussionForumComments");
        }
    }
}
