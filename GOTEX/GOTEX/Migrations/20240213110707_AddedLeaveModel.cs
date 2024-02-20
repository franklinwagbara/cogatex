using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class AddedLeaveModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastJobDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeclarationForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuarterId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ExportVolume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CrudeTheft = table.Column<int>(type: "int", nullable: false),
                    ExportProceedings = table.Column<int>(type: "int", nullable: false),
                    Bribe = table.Column<int>(type: "int", nullable: false),
                    StaffBribe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutstandingFee = table.Column<int>(type: "int", nullable: false),
                    Offence = table.Column<int>(type: "int", nullable: false),
                    Violation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclarationForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclarationForms_Quarters_QuarterId",
                        column: x => x.QuarterId,
                        principalTable: "Quarters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermitId = table.Column<int>(type: "int", nullable: false),
                    Effectiveness = table.Column<int>(type: "int", nullable: false),
                    IssuanceTime = table.Column<int>(type: "int", nullable: false),
                    PaymentProcess = table.Column<int>(type: "int", nullable: false),
                    DocumentUpload = table.Column<int>(type: "int", nullable: false),
                    KnowledgeGain = table.Column<int>(type: "int", nullable: false),
                    UserFriendly = table.Column<int>(type: "int", nullable: false),
                    PermitProcess = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surveys_Permits_PermitId",
                        column: x => x.PermitId,
                        principalTable: "Permits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationForms_QuarterId",
                table: "DeclarationForms",
                column: "QuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_PermitId",
                table: "Surveys",
                column: "PermitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclarationForms");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "LastJobDate",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Permits_ApplicationId",
                table: "Permits",
                column: "ApplicationId");
        }
    }
}
