using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemediagnoses_diagnoses_diagnosesid",
                table: "schemediagnoses");

            migrationBuilder.DropForeignKey(
                name: "fk_schemediagnoses_schemes_schemesid",
                table: "schemediagnoses");

            migrationBuilder.DropIndex(
                name: "IX_schemediagnoses_diagnosesid",
                table: "schemediagnoses");

            migrationBuilder.DropIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "schemediagnoses",
                newName: "tariff");

            migrationBuilder.AddColumn<string>(
                name: "icdcode",
                table: "schemediagnoses",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "icdversion",
                table: "schemediagnoses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "icdcode",
                table: "schemediagnoses");

            migrationBuilder.DropColumn(
                name: "icdversion",
                table: "schemediagnoses");

            migrationBuilder.RenameColumn(
                name: "tariff",
                table: "schemediagnoses",
                newName: "price");

            migrationBuilder.CreateIndex(
                name: "IX_schemediagnoses_diagnosesid",
                table: "schemediagnoses",
                column: "diagnosesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses",
                column: "schemesid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemediagnoses_diagnoses_diagnosesid",
                table: "schemediagnoses",
                column: "diagnosesid",
                principalTable: "diagnoses",
                principalColumn: "diagnosesid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemediagnoses_schemes_schemesid",
                table: "schemediagnoses",
                column: "schemesid",
                principalTable: "schemes",
                principalColumn: "schemesid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
