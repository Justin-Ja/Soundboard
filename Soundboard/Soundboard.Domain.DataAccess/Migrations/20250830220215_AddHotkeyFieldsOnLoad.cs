using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundboard.Domain.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHotkeyFieldsOnLoad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoundKeyCode",
                table: "SoundButtons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoundKeyModifiers",
                table: "SoundButtons",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundKeyCode",
                table: "SoundButtons");

            migrationBuilder.DropColumn(
                name: "BoundKeyModifiers",
                table: "SoundButtons");
        }
    }
}
