using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureProject.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColumnTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "tags");
        }
    }
}
