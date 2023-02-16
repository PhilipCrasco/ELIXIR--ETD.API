using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createordering : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrasactId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customercode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DateNeeded = table.Column<DateTime>(type: "Date", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemdDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantityOrdered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReject = table.Column<bool>(type: "bit", nullable: true),
                    RejectBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrepared = table.Column<bool>(type: "bit", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: true),
                    IsCancelBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNoPKey = table.Column<int>(type: "int", nullable: false),
                    IsMove = table.Column<bool>(type: "bit", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
