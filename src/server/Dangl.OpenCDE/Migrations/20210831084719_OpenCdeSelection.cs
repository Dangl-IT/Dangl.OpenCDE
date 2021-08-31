using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dangl.OpenCDE.Migrations
{
    public partial class OpenCdeSelection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenCdeDocumentSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newsequentialid()"),
                    DocumentId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCdeDocumentSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentSelections_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentSelections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenCdeDocumentSelectionSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newsequentialid()"),
                    UserId = table.Column<Guid>(nullable: false),
                    ValidUntilUtc = table.Column<DateTimeOffset>(nullable: false),
                    ClientState = table.Column<string>(nullable: false),
                    ClientCallbackUrl = table.Column<string>(nullable: false),
                    AuthenticationInformationJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCdeDocumentSelectionSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentSelectionSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentSelections_DocumentId",
                table: "OpenCdeDocumentSelections",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentSelections_UserId",
                table: "OpenCdeDocumentSelections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentSelectionSessions_UserId",
                table: "OpenCdeDocumentSelectionSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenCdeDocumentSelections");

            migrationBuilder.DropTable(
                name: "OpenCdeDocumentSelectionSessions");
        }
    }
}
