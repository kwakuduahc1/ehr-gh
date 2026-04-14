using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemediagnoses_schemes_schemesid",
                table: "schemediagnoses");

            migrationBuilder.DropIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses");

            migrationBuilder.AddColumn<string>(
                name: "variation",
                table: "schemediagnoses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "variation",
                table: "schemediagnoses");

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
    }
}
