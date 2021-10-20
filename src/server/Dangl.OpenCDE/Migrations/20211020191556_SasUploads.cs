using Microsoft.EntityFrameworkCore.Migrations;

namespace Dangl.OpenCDE.Migrations
{
    public partial class SasUploads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FileAvailableInStorage",
                table: "Files",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAvailableInStorage",
                table: "Files");
        }
    }
}
