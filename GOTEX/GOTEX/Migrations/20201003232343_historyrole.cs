using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class historyrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "ApplicationHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "ApplicationHistories");
        }
    }
}
