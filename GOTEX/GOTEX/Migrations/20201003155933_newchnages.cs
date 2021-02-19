using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class newchnages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDeskId",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "LastAssignedUser",
                table: "Applications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageId",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStageId",
                table: "ApplicationHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextStageId",
                table: "ApplicationHistories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAssignedUser",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "StageId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "CurrentStageId",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "NextStageId",
                table: "ApplicationHistories");

            migrationBuilder.AddColumn<int>(
                name: "CurrentDeskId",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
