using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    Uid = table.Column<string>(nullable: false),
                    Shortid = table.Column<long>(nullable: false),
                    BookmarkType = table.Column<int>(nullable: false),
                    ReplayCount = table.Column<long>(nullable: false),
                    FollowerCount = table.Column<long>(nullable: false),
                    FollowingCount = table.Column<long>(nullable: false),
                    Signature = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Face = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "LivemeAuthentication",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SsoToken = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Sid = table.Column<string>(nullable: true),
                    Tuid = table.Column<string>(nullable: true),
                    LoginTimestamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivemeAuthentication", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSeen",
                columns: table => new
                {
                    Uid = table.Column<string>(nullable: false),
                    Seen = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSeen", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    InternalId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.InternalId);
                });

            migrationBuilder.CreateTable(
                name: "LastScanResult",
                columns: table => new
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
                        name: "FK_LastScanResult_Bookmarks_BookmarkUid",
                        column: x => x.BookmarkUid,
                        principalTable: "Bookmarks",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Replays",
                columns: table => new
                {
                    VId = table.Column<string>(nullable: false),
                    VideoUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    FromUserInternalId = table.Column<int>(nullable: true),
                    Watched = table.Column<bool>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    StartTimeStamp = table.Column<long>(nullable: false),
                    ShareNum = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    Downloaded = table.Column<bool>(nullable: false),
                    Liked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replays", x => x.VId);
                    table.ForeignKey(
                        name: "FK_Replays_User_FromUserInternalId",
                        column: x => x.FromUserInternalId,
                        principalTable: "User",
                        principalColumn: "InternalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LastScanResult_BookmarkUid",
                table: "LastScanResult",
                column: "BookmarkUid");

            migrationBuilder.CreateIndex(
                name: "IX_Replays_FromUserInternalId",
                table: "Replays",
                column: "FromUserInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UId",
                table: "User",
                column: "UId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastScanResult");

            migrationBuilder.DropTable(
                name: "LivemeAuthentication");

            migrationBuilder.DropTable(
                name: "ProfileSeen");

            migrationBuilder.DropTable(
                name: "Replays");

            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
