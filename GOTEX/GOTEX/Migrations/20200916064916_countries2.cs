using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class countries2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryNane",
                table: "Nationalities");

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "Nationalities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "Nationalities");

            migrationBuilder.AddColumn<string>(
                name: "CountryNane",
                table: "Nationalities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
