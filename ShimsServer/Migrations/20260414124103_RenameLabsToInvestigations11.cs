using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsid",
                table: "schemeinvestigations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations");

            migrationBuilder.DropIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsid",
                table: "schemeinvestigations");

            migrationBuilder.RenameColumn(
                name: "schemelabsid",
                table: "schemeinvestigations",
                newName: "investigationsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations",
                column: "schemeinvestigationsid");

            migrationBuilder.CreateIndex(
                name: "IX_schemeinvestigations_investigationsid",
                table: "schemeinvestigations",
                column: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemeinvestigations",
                principalColumn: "schemeinvestigationsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_investigationsid",
                table: "schemeinvestigations",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_investigationsid",
                table: "schemeinvestigations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations");

            migrationBuilder.DropIndex(
                name: "IX_schemeinvestigations_investigationsid",
                table: "schemeinvestigations");

            migrationBuilder.RenameColumn(
                name: "investigationsid",
                table: "schemeinvestigations",
                newName: "schemelabsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations",
                column: "schemelabsid");

            migrationBuilder.CreateIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsid",
                table: "schemeinvestigations",
                column: "schemeinvestigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemeinvestigations",
                principalColumn: "schemelabsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsid",
                table: "schemeinvestigations",
                column: "schemeinvestigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
