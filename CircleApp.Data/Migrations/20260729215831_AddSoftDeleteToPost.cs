using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "posts");
        }
    }
}
