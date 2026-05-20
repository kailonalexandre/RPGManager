using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campaign_notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    ContentMarkdown = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: false),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Visibility = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LinkedEntityType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    LinkedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalProvider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_notes_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign_notes_users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_CampaignId",
                table: "campaign_notes",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_LinkedEntityId",
                table: "campaign_notes",
                column: "LinkedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_LinkedEntityType",
                table: "campaign_notes",
                column: "LinkedEntityType");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_OwnerUserId",
                table: "campaign_notes",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_Title",
                table: "campaign_notes",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_notes_Visibility",
                table: "campaign_notes",
                column: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_notes");
        }
    }
}
