using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class NHISDrugsFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "schemedrugs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dosageform",
                table: "schemedrugs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                table: "schemedrugs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "prescriptionlevel",
                table: "schemedrugs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pricingunit",
                table: "schemedrugs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "strength",
                table: "schemedrugs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "schemedrugs");

            migrationBuilder.DropColumn(
                name: "dosageform",
                table: "schemedrugs");

            migrationBuilder.DropColumn(
                name: "isactive",
                table: "schemedrugs");

            migrationBuilder.DropColumn(
                name: "prescriptionlevel",
                table: "schemedrugs");

            migrationBuilder.DropColumn(
                name: "pricingunit",
                table: "schemedrugs");

            migrationBuilder.DropColumn(
                name: "strength",
                table: "schemedrugs");
        }
    }
}
