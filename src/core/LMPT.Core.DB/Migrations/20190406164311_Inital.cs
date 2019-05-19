using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Bookmarks",
                table => new
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
                    Nickname = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Bookmarks", x => x.Uid); });

            migrationBuilder.CreateTable(
                "LivemeAuthentication",
                table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SsoToken = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Sid = table.Column<string>(nullable: true),
                    Tuid = table.Column<string>(nullable: true),
                    LoginTimestamp = table.Column<long>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_LivemeAuthentication", x => x.ID); });

            migrationBuilder.CreateTable(
                "ProfileSeen",
                table => new
                {
                    Uid = table.Column<string>(nullable: false),
                    Seen = table.Column<long>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ProfileSeen", x => x.Uid); });

            migrationBuilder.CreateTable(
                "User",
                table => new
                {
                    InternalId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_User", x => x.InternalId); });

            migrationBuilder.CreateTable(
                "Replays",
                table => new
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
                    Downloaded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replays", x => x.VId);
                    table.ForeignKey(
                        "FK_Replays_User_FromUserInternalId",
                        x => x.FromUserInternalId,
                        "User",
                        "InternalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Replays_FromUserInternalId",
                "Replays",
                "FromUserInternalId");

            migrationBuilder.CreateIndex(
                "IX_User_UId",
                "User",
                "UId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Bookmarks");

            migrationBuilder.DropTable(
                "LivemeAuthentication");

            migrationBuilder.DropTable(
                "ProfileSeen");

            migrationBuilder.DropTable(
                "Replays");

            migrationBuilder.DropTable(
                "User");
        }
    }
}