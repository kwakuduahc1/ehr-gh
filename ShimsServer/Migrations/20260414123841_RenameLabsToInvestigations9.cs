using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsgro~",
                table: "schemeinvestigations");

            migrationBuilder.RenameColumn(
                name: "schemeinvestigationsgroupid",
                table: "schemeinvestigations",
                newName: "schemeinvestigationsid");

            migrationBuilder.RenameIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsgroupid",
                table: "schemeinvestigations",
                newName: "IX_schemeinvestigations_schemeinvestigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsid",
                table: "schemeinvestigations",
                column: "schemeinvestigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsid",
                table: "schemeinvestigations");

            migrationBuilder.RenameColumn(
                name: "schemeinvestigationsid",
                table: "schemeinvestigations",
                newName: "schemeinvestigationsgroupid");

            migrationBuilder.RenameIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsid",
                table: "schemeinvestigations",
                newName: "IX_schemeinvestigations_schemeinvestigationsgroupid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsgro~",
                table: "schemeinvestigations",
                column: "schemeinvestigationsgroupid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
