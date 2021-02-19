using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class appyear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionRole",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Applications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationType",
                table: "Applications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "ActionRole",
                table: "WorkFlows");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationType",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
