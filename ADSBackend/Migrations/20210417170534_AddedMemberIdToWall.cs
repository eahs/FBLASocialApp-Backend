using Microsoft.EntityFrameworkCore.Migrations;

namespace YakkaApp.Migrations
{
    public partial class AddedMemberIdToWall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Wall_WallId",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_WallId",
                table: "Member");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Wall",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WallId1",
                table: "Member",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_WallId1",
                table: "Member",
                column: "WallId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Wall_WallId1",
                table: "Member",
                column: "WallId1",
                principalTable: "Wall",
                principalColumn: "WallId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Wall_WallId1",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_WallId1",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Wall");

            migrationBuilder.DropColumn(
                name: "WallId1",
                table: "Member");

            migrationBuilder.CreateIndex(
                name: "IX_Member_WallId",
                table: "Member",
                column: "WallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Wall_WallId",
                table: "Member",
                column: "WallId",
                principalTable: "Wall",
                principalColumn: "WallId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
