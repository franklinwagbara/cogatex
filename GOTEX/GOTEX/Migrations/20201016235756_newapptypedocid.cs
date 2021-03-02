using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class newapptypedocid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocTypeId",
                table: "ApplicationDocuments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ManualRemitaValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RRR = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    Beneficiary = table.Column<string>(nullable: true),
                    FundingBank = table.Column<string>(nullable: true),
                    PaymentSource = table.Column<string>(nullable: true),
                    NetAmount = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    AddedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualRemitaValues", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualRemitaValues");

            migrationBuilder.DropColumn(
                name: "DocTypeId",
                table: "ApplicationDocuments");
        }
    }
}
