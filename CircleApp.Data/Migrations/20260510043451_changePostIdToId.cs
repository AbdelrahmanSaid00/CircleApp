using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleApp.Migrations
{
    /// <inheritdoc />
    public partial class changePostIdToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "postId",
                table: "posts",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "posts",
                newName: "postId");
        }
    }
}
