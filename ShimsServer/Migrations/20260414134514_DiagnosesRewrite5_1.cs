using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite5_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses",
                column: "schemesid");

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
                name: "fk_schemediagnoses_schemes_schemesid",
                table: "schemediagnoses");

            migrationBuilder.DropIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses");
        }
    }
}
