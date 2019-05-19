using Microsoft.EntityFrameworkCore.Migrations;

namespace LMPT.DB.Migrations
{
    public partial class addedLikeToReplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "Liked",
                "Replays",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Liked",
                "Replays");
        }
    }
}