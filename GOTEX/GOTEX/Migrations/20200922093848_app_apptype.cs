using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class app_apptype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationTye",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationType",
                table: "Applications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationType",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationTye",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
