using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class apptable_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Quarter",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Terminal",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuarterId",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TerminalId",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ProductId",
                table: "Applications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_QuarterId",
                table: "Applications",
                column: "QuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_TerminalId",
                table: "Applications",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Products_ProductId",
                table: "Applications",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Quarters_QuarterId",
                table: "Applications",
                column: "QuarterId",
                principalTable: "Quarters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Terminals_TerminalId",
                table: "Applications",
                column: "TerminalId",
                principalTable: "Terminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Products_ProductId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Quarters_QuarterId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Terminals_TerminalId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ProductId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_QuarterId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_TerminalId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "QuarterId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quarter",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Terminal",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
