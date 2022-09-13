using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dangl.OpenCDE.Migrations
{
    public partial class OpenCDEUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenCdeDocumentSelectionSessions");

            migrationBuilder.CreateTable(
                name: "OpenCdeDocumentDownloadSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newsequentialid()"),
                    UserId = table.Column<Guid>(nullable: false),
                    ValidUntilUtc = table.Column<DateTimeOffset>(nullable: false),
                    ClientCallbackUrl = table.Column<string>(nullable: false),
                    AuthenticationInformationJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCdeDocumentDownloadSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentDownloadSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenCdeDocumentUploadSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newsequentialid()"),
                    UserId = table.Column<Guid>(nullable: false),
                    ValidUntilUtc = table.Column<DateTimeOffset>(nullable: false),
                    ClientCallbackUrl = table.Column<string>(nullable: false),
                    AuthenticationInformationJson = table.Column<string>(nullable: true),
                    SelectedProjectId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCdeDocumentUploadSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentUploadSessions_Projects_SelectedProjectId",
                        column: x => x.SelectedProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpenCdeDocumentUploadSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingOpenCdeUploadFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newsequentialid()"),
                    UploadSessionId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    SessionFileId = table.Column<string>(nullable: false),
                    LinkedCdeDocumentId = table.Column<Guid>(nullable: true),
                    IsCancelled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingOpenCdeUploadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingOpenCdeUploadFiles_Documents_LinkedCdeDocumentId",
                        column: x => x.LinkedCdeDocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PendingOpenCdeUploadFiles_OpenCdeDocumentUploadSessions_UploadSessionId",
                        column: x => x.UploadSessionId,
                        principalTable: "OpenCdeDocumentUploadSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentDownloadSessions_UserId",
                table: "OpenCdeDocumentDownloadSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentUploadSessions_SelectedProjectId",
                table: "OpenCdeDocumentUploadSessions",
                column: "SelectedProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenCdeDocumentUploadSessions_UserId",
                table: "OpenCdeDocumentUploadSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingOpenCdeUploadFiles_LinkedCdeDocumentId",
                table: "PendingOpenCdeUploadFiles",
                column: "LinkedCdeDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingOpenCdeUploadFiles_UploadSessionId",
                table: "PendingOpenCdeUploadFiles",
                column: "UploadSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenCdeDocumentDownloadSessions");

            migrationBuilder.DropTable(
                name: "PendingOpenCdeUploadFiles");

            migrationBuilder.DropTable(
                name: "OpenCdeDocumentUploadSessions");

            migrationBuilder.CreateTable(
                name: "OpenCdeDocumentSelectionSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    AuthenticationInformationJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientCallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidUntilUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                name: "IX_OpenCdeDocumentSelectionSessions_UserId",
                table: "OpenCdeDocumentSelectionSessions",
                column: "UserId");
        }
    }
}
