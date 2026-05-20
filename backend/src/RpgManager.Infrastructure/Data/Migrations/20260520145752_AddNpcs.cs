using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNpcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "npcs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Alias = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Race = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Occupation = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Location = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Faction = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Personality = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Appearance = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Motivation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Secrets = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    StatBlockJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsImportant = table.Column<bool>(type: "boolean", nullable: false),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false),
                    Visibility = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npcs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_npcs_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_npcs_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_npcs_CampaignId",
                table: "npcs",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_npcs_CreatedByUserId",
                table: "npcs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_npcs_Faction",
                table: "npcs",
                column: "Faction");

            migrationBuilder.CreateIndex(
                name: "IX_npcs_Location",
                table: "npcs",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_npcs_Name",
                table: "npcs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_npcs_Visibility",
                table: "npcs",
                column: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "npcs");
        }
    }
}
