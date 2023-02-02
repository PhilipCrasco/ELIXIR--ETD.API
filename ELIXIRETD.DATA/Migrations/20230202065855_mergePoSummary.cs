using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class mergePoSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelBy",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "PoSummaries",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MiscellaneousIssueDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreparedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WareHouseId = table.Column<int>(type: "int", nullable: false),
                    IssuePKey = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsTransact = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscellaneousIssueDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MiscellaneousReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreparedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscellaneousReceipts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiscellaneousIssueDetail");

            migrationBuilder.DropTable(
                name: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "CancelBy",
                table: "PoSummaries");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "PoSummaries");
        }
    }
}
