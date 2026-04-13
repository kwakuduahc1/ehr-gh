using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DrugLens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "schemedrugs",
                newName: "drugcode");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "drugs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_schemedrugs_drugcode_dateset_dosageform_strength_pricingunit",
                table: "schemedrugs",
                columns: new[] { "drugcode", "dateset", "dosageform", "strength", "pricingunit" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_schemedrugs_drugcode_dateset_dosageform_strength_pricingunit",
                table: "schemedrugs");

            migrationBuilder.RenameColumn(
                name: "drugcode",
                table: "schemedrugs",
                newName: "code");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "drugs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
