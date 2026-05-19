using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterAbilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Charisma",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CharismaSaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CharismaSaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Constitution",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConstitutionSaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ConstitutionSaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Dexterity",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DexteritySaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DexteritySaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Intelligence",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntelligenceSaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IntelligenceSaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StrengthSaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "StrengthSaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Wisdom",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WisdomSaveCustomBonus",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WisdomSaveProficient",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charisma",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "CharismaSaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "CharismaSaveProficient",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "Constitution",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "ConstitutionSaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "ConstitutionSaveProficient",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "Dexterity",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "DexteritySaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "DexteritySaveProficient",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "Intelligence",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "IntelligenceSaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "IntelligenceSaveProficient",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "StrengthSaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "StrengthSaveProficient",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "Wisdom",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "WisdomSaveCustomBonus",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "WisdomSaveProficient",
                table: "characters");
        }
    }
}
