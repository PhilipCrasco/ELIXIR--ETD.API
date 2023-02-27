using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddBorrowedConsume : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Consume",
                table: "BorrowedReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnQuantity",
                table: "BorrowedReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Consume",
                table: "BorrowedIssueDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnQuantity",
                table: "BorrowedIssueDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consume",
                table: "BorrowedReceipts");

            migrationBuilder.DropColumn(
                name: "ReturnQuantity",
                table: "BorrowedReceipts");

            migrationBuilder.DropColumn(
                name: "Consume",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "ReturnQuantity",
                table: "BorrowedIssueDetails");
        }
    }
}
