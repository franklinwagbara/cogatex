using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class Workfloachnage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "processingLocataion",
                table: "WorkFlows",
                newName: "ProcessingLocataion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessingLocataion",
                table: "WorkFlows",
                newName: "processingLocataion");
        }
    }
}
