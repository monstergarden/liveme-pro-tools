using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class deletedFlagBookmarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "Deleted",
                "Bookmarks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Deleted",
                "Bookmarks");
        }
    }
}