using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavsFix7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests");

            migrationBuilder.RenameColumn(
                name: "schemelabsid",
                table: "investigationsrequests",
                newName: "investigationsid");

            migrationBuilder.RenameIndex(
                name: "IX_investigationsrequests_schemelabsid",
                table: "investigationsrequests",
                newName: "IX_investigationsrequests_investigationsid");

            migrationBuilder.AddColumn<Guid>(
                name: "schemeinvestigationsid",
                table: "investigationsrequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_investigationsrequests_schemeinvestigationsid",
                table: "investigationsrequests",
                column: "schemeinvestigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_investigations_investigationsid",
                table: "investigationsrequests",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemeinvestiga~",
                table: "investigationsrequests",
                column: "schemeinvestigationsid",
                principalTable: "schemeinvestigations",
                principalColumn: "schemeinvestigationsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_investigations_investigationsid",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemeinvestiga~",
                table: "investigationsrequests");

            migrationBuilder.DropIndex(
                name: "IX_investigationsrequests_schemeinvestigationsid",
                table: "investigationsrequests");

            migrationBuilder.DropColumn(
                name: "schemeinvestigationsid",
                table: "investigationsrequests");

            migrationBuilder.RenameColumn(
                name: "investigationsid",
                table: "investigationsrequests",
                newName: "schemelabsid");

            migrationBuilder.RenameIndex(
                name: "IX_investigationsrequests_investigationsid",
                table: "investigationsrequests",
                newName: "IX_investigationsrequests_schemelabsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemeinvestigations_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemeinvestigations",
                principalColumn: "schemeinvestigationsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
