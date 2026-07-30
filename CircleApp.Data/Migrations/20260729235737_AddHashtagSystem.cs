using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHashtagSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hashtags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hashtags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HashtagPost",
                columns: table => new
                {
                    HashtagsId = table.Column<int>(type: "int", nullable: false),
                    PostsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HashtagPost", x => new { x.HashtagsId, x.PostsId });
                    table.ForeignKey(
                        name: "FK_HashtagPost_hashtags_HashtagsId",
                        column: x => x.HashtagsId,
                        principalTable: "hashtags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HashtagPost_posts_PostsId",
                        column: x => x.PostsId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HashtagPost_PostsId",
                table: "HashtagPost",
                column: "PostsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HashtagPost");

            migrationBuilder.DropTable(
                name: "hashtags");
        }
    }
}
