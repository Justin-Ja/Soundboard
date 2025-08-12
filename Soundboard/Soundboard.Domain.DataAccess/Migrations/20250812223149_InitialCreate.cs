using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundboard.Domain.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GridLayouts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 127, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridLayouts", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "SoundButtons",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    HotKey = table.Column<string>(type: "TEXT", nullable: true),
                    GridId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoundButtons", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_SoundButtons_GridLayouts_GridId",
                        column: x => x.GridId,
                        principalTable: "GridLayouts",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoundButtons_GridId",
                table: "SoundButtons",
                column: "GridId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoundButtons");

            migrationBuilder.DropTable(
                name: "GridLayouts");
        }
    }
}
