using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class mergeWarehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualGood",
                table: "WarehouseReceived",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualReceivingDate",
                table: "WarehouseReceived",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmRejectByWarehouse",
                table: "WarehouseReceived",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWarehouseReceived",
                table: "WarehouseReceived",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReject",
                table: "WarehouseReceived",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "WarehouseReject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseReceivingId = table.Column<int>(type: "int", nullable: false),
                    RejectedDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RejectedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseReject", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseReject");

            migrationBuilder.DropColumn(
                name: "ActualGood",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "ActualReceivingDate",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "ConfirmRejectByWarehouse",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "IsWarehouseReceived",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "TotalReject",
                table: "WarehouseReceived");
        }
    }
}
