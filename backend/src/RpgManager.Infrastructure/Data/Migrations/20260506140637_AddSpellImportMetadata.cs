using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellImportMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "spells",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSource",
                table: "spells",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedAt",
                table: "spells",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImported",
                table: "spells",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSrd",
                table: "spells",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "spells",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pt-BR");

            migrationBuilder.AddColumn<string>(
                name: "RulesVersion",
                table: "spells",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "spells",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TranslationMissing",
                table: "spells",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_spells_ExternalSource_ExternalId",
                table: "spells",
                columns: new[] { "ExternalSource", "ExternalId" },
                unique: true,
                filter: "\"ExternalSource\" IS NOT NULL AND \"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_spells_IsImported",
                table: "spells",
                column: "IsImported");

            migrationBuilder.CreateIndex(
                name: "IX_spells_IsSrd",
                table: "spells",
                column: "IsSrd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_spells_ExternalSource_ExternalId",
                table: "spells");

            migrationBuilder.DropIndex(
                name: "IX_spells_IsImported",
                table: "spells");

            migrationBuilder.DropIndex(
                name: "IX_spells_IsSrd",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "ExternalSource",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "ImportedAt",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "IsImported",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "IsSrd",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "RulesVersion",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "TranslationMissing",
                table: "spells");
        }
    }
}
