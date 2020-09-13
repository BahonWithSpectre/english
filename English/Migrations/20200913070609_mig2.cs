using Microsoft.EntityFrameworkCore.Migrations;

namespace english.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Free",
                table: "KursVideos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WorkUrl",
                table: "KursVideos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Free",
                table: "KursVideos");

            migrationBuilder.DropColumn(
                name: "WorkUrl",
                table: "KursVideos");
        }
    }
}
