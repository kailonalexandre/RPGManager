using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterBuilderOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BackgroundId",
                table: "characters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "characters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RaceId",
                table: "characters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "backgrounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Source = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsHomebrew = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_backgrounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_backgrounds_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    HitDie = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Source = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsHomebrew = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_classes_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "races",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Source = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsHomebrew = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_races", x => x.Id);
                    table.ForeignKey(
                        name: "FK_races_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_BackgroundId",
                table: "characters",
                column: "BackgroundId");

            migrationBuilder.CreateIndex(
                name: "IX_characters_ClassId",
                table: "characters",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_characters_RaceId",
                table: "characters",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_backgrounds_CreatedByUserId",
                table: "backgrounds",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_backgrounds_Name",
                table: "backgrounds",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_character_classes_CreatedByUserId",
                table: "character_classes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_character_classes_Name",
                table: "character_classes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_races_CreatedByUserId",
                table: "races",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_races_Name",
                table: "races",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_characters_backgrounds_BackgroundId",
                table: "characters",
                column: "BackgroundId",
                principalTable: "backgrounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_characters_character_classes_ClassId",
                table: "characters",
                column: "ClassId",
                principalTable: "character_classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_characters_races_RaceId",
                table: "characters",
                column: "RaceId",
                principalTable: "races",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_characters_backgrounds_BackgroundId",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_characters_character_classes_ClassId",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_characters_races_RaceId",
                table: "characters");

            migrationBuilder.DropTable(
                name: "backgrounds");

            migrationBuilder.DropTable(
                name: "character_classes");

            migrationBuilder.DropTable(
                name: "races");

            migrationBuilder.DropIndex(
                name: "IX_characters_BackgroundId",
                table: "characters");

            migrationBuilder.DropIndex(
                name: "IX_characters_ClassId",
                table: "characters");

            migrationBuilder.DropIndex(
                name: "IX_characters_RaceId",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "BackgroundId",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "RaceId",
                table: "characters");
        }
    }
}
