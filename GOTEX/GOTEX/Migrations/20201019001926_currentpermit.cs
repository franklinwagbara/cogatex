using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class currentpermit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "LicenName",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Permits");

            migrationBuilder.AddColumn<int>(
                name: "ElpsId",
                table: "Permits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IsRenewed",
                table: "Permits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseName",
                table: "Permits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermitNumber",
                table: "Permits",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Printed",
                table: "Permits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Permits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentPermit",
                table: "Applications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permits_Applications_ApplicationId",
                table: "Permits",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permits_Applications_ApplicationId",
                table: "Permits");

            migrationBuilder.DropIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "ElpsId",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "IsRenewed",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "LicenseName",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "PermitNumber",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "Printed",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "CurrentPermit",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Permits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LicenName",
                table: "Permits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Permits",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
