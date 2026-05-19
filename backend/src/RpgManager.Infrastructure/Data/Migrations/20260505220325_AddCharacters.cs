using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TokenImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TotalLevel = table.Column<int>(type: "integer", nullable: false),
                    Species = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    MainClass = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Subclass = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Background = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Alignment = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    Inspiration = table.Column<bool>(type: "boolean", nullable: false),
                    ProficiencyBonus = table.Column<int>(type: "integer", nullable: false),
                    ArmorClass = table.Column<int>(type: "integer", nullable: false),
                    Initiative = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "integer", nullable: false),
                    CurrentHitPoints = table.Column<int>(type: "integer", nullable: false),
                    TemporaryHitPoints = table.Column<int>(type: "integer", nullable: false),
                    TotalHitDice = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    AvailableHitDice = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    PhysicalDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PersonalityTraits = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    Ideals = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    Bonds = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    Flaws = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    Backstory = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    QuickNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_characters_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_characters_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_CampaignId",
                table: "characters",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_characters_UserId",
                table: "characters",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters");
        }
    }
}
