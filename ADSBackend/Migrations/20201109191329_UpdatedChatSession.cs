using Microsoft.EntityFrameworkCore.Migrations;

namespace YakkaApp.Migrations
{
    public partial class UpdatedChatSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Member");

            migrationBuilder.AddColumn<string>(
                name: "ChatPrivateKey",
                table: "ChatSession",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatPrivateKey",
                table: "ChatSession");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Member",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
