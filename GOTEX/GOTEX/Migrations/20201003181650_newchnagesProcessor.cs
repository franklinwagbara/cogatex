using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class newchnagesProcessor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAssignedUser",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "LastAssignedUserId",
                table: "Applications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAssignedUserId",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "LastAssignedUser",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
