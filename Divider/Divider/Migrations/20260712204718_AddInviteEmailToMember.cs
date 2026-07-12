using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Divider.Migrations
{
    /// <inheritdoc />
    public partial class AddInviteEmailToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteEmail",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteEmail",
                table: "Members");
        }
    }
}
