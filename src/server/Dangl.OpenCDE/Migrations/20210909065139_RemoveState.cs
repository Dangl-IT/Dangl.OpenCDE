using Microsoft.EntityFrameworkCore.Migrations;

namespace Dangl.OpenCDE.Migrations
{
    public partial class RemoveState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientState",
                table: "OpenCdeDocumentSelectionSessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientState",
                table: "OpenCdeDocumentSelectionSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
