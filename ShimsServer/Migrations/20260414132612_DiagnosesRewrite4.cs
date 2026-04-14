using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
