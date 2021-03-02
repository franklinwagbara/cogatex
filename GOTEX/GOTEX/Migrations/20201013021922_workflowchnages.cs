using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class workflowchnages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationHistories_AspNetUsers_AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationHistories_AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ActionRole",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "CureentStageId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "NextStageId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "ProcessingLocataion",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "CurrentStageId",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "NextStageId",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ProcessingLocation",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "ApplicationHistories");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TriggeredByRole",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentUser",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentUserRole",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingUser",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingUserRole",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkFlowId",
                table: "ApplicationHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PaymentApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(nullable: false),
                    Bank = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    PaymentUrl = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentApprovals_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentApprovals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApprovals_ApplicationId",
                table: "PaymentApprovals",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApprovals_UserId",
                table: "PaymentApprovals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentApprovals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "TriggeredByRole",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "CurrentUser",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "CurrentUserRole",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ProcessingUser",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ProcessingUserRole",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "WorkFlowId",
                table: "ApplicationHistories");

            migrationBuilder.AddColumn<string>(
                name: "ActionRole",
                table: "WorkFlows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CureentStageId",
                table: "WorkFlows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextStageId",
                table: "WorkFlows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingLocataion",
                table: "WorkFlows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "ApplicationHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStageId",
                table: "ApplicationHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextStageId",
                table: "ApplicationHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingLocation",
                table: "ApplicationHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "ApplicationHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_AssignedTo",
                table: "ApplicationHistories",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationHistories_AspNetUsers_AssignedTo",
                table: "ApplicationHistories",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
