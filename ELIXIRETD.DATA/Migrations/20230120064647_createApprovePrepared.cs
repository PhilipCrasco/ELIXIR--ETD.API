using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createApprovePrepared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedDate",
                table: "Orders",
                newName: "SyncDate");

            migrationBuilder.AddColumn<string>(
                name: "AddedBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedBy",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "SyncDate",
                table: "Orders",
                newName: "ReceivedDate");
        }
    }
}
