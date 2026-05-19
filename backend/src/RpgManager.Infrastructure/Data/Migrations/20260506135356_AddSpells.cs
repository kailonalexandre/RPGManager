using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpells : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "spells",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    School = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CastingTime = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Range = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Components = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Material = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Duration = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    IsConcentration = table.Column<bool>(type: "boolean", nullable: false),
                    IsRitual = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    HigherLevelDescription = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    AvailableClasses = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsHomebrew = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Visibility = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_spells_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_spells_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_spells_CampaignId",
                table: "spells",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_spells_CreatedByUserId",
                table: "spells",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_spells_Level",
                table: "spells",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_spells_Name",
                table: "spells",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_spells_School",
                table: "spells",
                column: "School");

            migrationBuilder.CreateIndex(
                name: "IX_spells_Visibility",
                table: "spells",
                column: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "spells");
        }
    }
}
