using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class lastupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                "LastUpdated",
                "Bookmarks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "LastUpdated",
                "Bookmarks");
        }
    }
}