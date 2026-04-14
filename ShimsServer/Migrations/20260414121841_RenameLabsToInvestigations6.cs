using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemelabs_schemelabsid",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_schemelabs_investigations_investigationsid",
                table: "schemelabs");

            migrationBuilder.DropForeignKey(
                name: "fk_schemelabs_schemes_schemesid",
                table: "schemelabs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_schemelabs",
                table: "schemelabs");

            migrationBuilder.RenameTable(
                name: "schemelabs",
                newName: "schemeinvestigations");

            migrationBuilder.RenameIndex(
                name: "IX_schemelabs_schemesid",
                table: "schemeinvestigations",
                newName: "IX_schemeinvestigations_schemesid");

            migrationBuilder.RenameIndex(
                name: "IX_schemelabs_investigationsid",
                table: "schemeinvestigations",
                newName: "IX_schemeinvestigations_investigationsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations",
                column: "schemelabsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemeinvestigations",
                principalColumn: "schemelabsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_investigationsid",
                table: "schemeinvestigations",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_schemes_schemesid",
                table: "schemeinvestigations",
                column: "schemesid",
                principalTable: "schemes",
                principalColumn: "schemesid",
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

            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_schemes_schemesid",
                table: "schemeinvestigations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_schemeinvestigations",
                table: "schemeinvestigations");

            migrationBuilder.RenameTable(
                name: "schemeinvestigations",
                newName: "schemelabs");

            migrationBuilder.RenameIndex(
                name: "IX_schemeinvestigations_schemesid",
                table: "schemelabs",
                newName: "IX_schemelabs_schemesid");

            migrationBuilder.RenameIndex(
                name: "IX_schemeinvestigations_investigationsid",
                table: "schemelabs",
                newName: "IX_schemelabs_investigationsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_schemelabs",
                table: "schemelabs",
                column: "schemelabsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemelabs_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemelabs",
                principalColumn: "schemelabsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemelabs_investigations_investigationsid",
                table: "schemelabs",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemelabs_schemes_schemesid",
                table: "schemelabs",
                column: "schemesid",
                principalTable: "schemes",
                principalColumn: "schemesid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
