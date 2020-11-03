using Microsoft.EntityFrameworkCore.Migrations;

namespace YakkaApp.Migrations
{
    public partial class AddedProfilePhotoToMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "profileImageSource",
                table: "Member");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePhotoPhotoId",
                table: "Member",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PostPhoto",
                columns: table => new
                {
                    PhotoId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPhoto", x => new { x.PostId, x.PhotoId });
                    table.ForeignKey(
                        name: "FK_PostPhoto_Photo_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photo",
                        principalColumn: "PhotoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostPhoto_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Member_ProfilePhotoPhotoId",
                table: "Member",
                column: "ProfilePhotoPhotoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPhoto_PhotoId",
                table: "PostPhoto",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Photo_ProfilePhotoPhotoId",
                table: "Member",
                column: "ProfilePhotoPhotoId",
                principalTable: "Photo",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Photo_ProfilePhotoPhotoId",
                table: "Member");

            migrationBuilder.DropTable(
                name: "PostPhoto");

            migrationBuilder.DropIndex(
                name: "IX_Member_ProfilePhotoPhotoId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoPhotoId",
                table: "Member");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Post",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profileImageSource",
                table: "Member",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
