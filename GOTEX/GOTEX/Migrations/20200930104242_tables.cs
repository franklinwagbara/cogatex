using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GOTEX.Migrations
{
    public partial class tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationTypeId",
                table: "WorkFlows",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CureentStageId",
                table: "WorkFlows",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextStageId",
                table: "WorkFlows",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "processingLocataion",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Submitted",
                table: "Applications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutoPushed",
                table: "ApplicationHistories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssigned",
                table: "ApplicationHistories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingLocation",
                table: "ApplicationHistories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Status = table.Column<string>(maxLength: 6, nullable: false),
                    PaymentCode = table.Column<string>(maxLength: 100, nullable: false),
                    PaymentType = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptNo = table.Column<string>(maxLength: 25, nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DatePaid = table.Column<DateTime>(nullable: false),
                    PaymentTransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemitaPayments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    TransactionDate = table.Column<string>(maxLength: 50, nullable: false),
                    ReferenceNumber = table.Column<string>(maxLength: 50, nullable: true),
                    OnlineReference = table.Column<string>(maxLength: 50, nullable: true),
                    PaymentReference = table.Column<string>(maxLength: 100, nullable: true),
                    ApprovedAmount = table.Column<string>(maxLength: 10, nullable: true),
                    ResponseDescription = table.Column<string>(maxLength: 100, nullable: true),
                    ResponseCode = table.Column<string>(maxLength: 50, nullable: true),
                    TransactionAmount = table.Column<string>(maxLength: 10, nullable: true),
                    TransactionCurrency = table.Column<string>(maxLength: 5, nullable: true),
                    CustomerName = table.Column<string>(maxLength: 200, nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(maxLength: 20, nullable: false),
                    PaymentLogId = table.Column<string>(maxLength: 50, nullable: true),
                    QueryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WebpayReference = table.Column<string>(nullable: true),
                    RRR = table.Column<string>(nullable: true),
                    PaymentSource = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemitaPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemitaPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_ApplicationTypeId",
                table: "WorkFlows",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_AssignedTo",
                table: "ApplicationHistories",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ApplicationId",
                table: "Invoices",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_RemitaPayments_UserId",
                table: "RemitaPayments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationHistories_AspNetUsers_AssignedTo",
                table: "ApplicationHistories",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlows_ApplicationTypes_ApplicationTypeId",
                table: "WorkFlows",
                column: "ApplicationTypeId",
                principalTable: "ApplicationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationHistories_AspNetUsers_AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlows_ApplicationTypes_ApplicationTypeId",
                table: "WorkFlows");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "RemitaPayments");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlows_ApplicationTypeId",
                table: "WorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationHistories_AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "CureentStageId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "NextStageId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "processingLocataion",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "Submitted",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "AutoPushed",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "IsAssigned",
                table: "ApplicationHistories");

            migrationBuilder.DropColumn(
                name: "ProcessingLocation",
                table: "ApplicationHistories");
        }
    }
}
