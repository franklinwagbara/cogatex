using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class GatexDbV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "PaymentApprovals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocType",
                table: "ApplicationTypeDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentEvidenceId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TerminalId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ElpsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facilities_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facilities_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facilities_Terminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentEvidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    ReferenceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationQuantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentEvidences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_FacilityId",
                table: "Applications",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PaymentEvidenceId",
                table: "Applications",
                column: "PaymentEvidenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_CompanyId",
                table: "Facilities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ProductId",
                table: "Facilities",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_TerminalId",
                table: "Facilities",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Facilities_FacilityId",
                table: "Applications",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_PaymentEvidences_PaymentEvidenceId",
                table: "Applications",
                column: "PaymentEvidenceId",
                principalTable: "PaymentEvidences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Facilities_FacilityId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_PaymentEvidences_PaymentEvidenceId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "PaymentEvidences");

            migrationBuilder.DropIndex(
                name: "IX_Applications_FacilityId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_PaymentEvidenceId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "PaymentApprovals");

            migrationBuilder.DropColumn(
                name: "DocType",
                table: "ApplicationTypeDocuments");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PaymentEvidenceId",
                table: "Applications");
        }
    }
}
