using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class AddScanResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "LastScanResult",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Delta = table.Column<int>(nullable: false),
                    ScanType = table.Column<int>(nullable: false),
                    BookmarkUid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastScanResult", x => x.Id);
                    table.ForeignKey(
                        "FK_LastScanResult_Bookmarks_BookmarkUid",
                        x => x.BookmarkUid,
                        "Bookmarks",
                        "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_LastScanResult_BookmarkUid",
                "LastScanResult",
                "BookmarkUid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LastScanResult");
        }
    }
}