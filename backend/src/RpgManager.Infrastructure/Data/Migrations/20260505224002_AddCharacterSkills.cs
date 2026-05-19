using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillType = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    BaseAttribute = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    IsProficient = table.Column<bool>(type: "boolean", nullable: false),
                    IsExpertise = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_skills_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_skills_CharacterId_SkillType",
                table: "character_skills",
                columns: new[] { "CharacterId", "SkillType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_skills");
        }
    }
}
