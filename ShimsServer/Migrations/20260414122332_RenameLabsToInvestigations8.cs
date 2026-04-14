using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_investigationsid",
                table: "schemeinvestigations");

            migrationBuilder.DropIndex(
                name: "IX_schemeinvestigations_investigationsid",
                table: "schemeinvestigations");

            migrationBuilder.DropColumn(
                name: "investigationsid",
                table: "schemeinvestigations");

            migrationBuilder.CreateIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsgroupid",
                table: "schemeinvestigations",
                column: "schemeinvestigationsgroupid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsgro~",
                table: "schemeinvestigations",
                column: "schemeinvestigationsgroupid",
                principalTable: "investigations",
                principalColumn: "investigationsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_schemeinvestigations_investigations_schemeinvestigationsgro~",
                table: "schemeinvestigations");

            migrationBuilder.DropIndex(
                name: "IX_schemeinvestigations_schemeinvestigationsgroupid",
                table: "schemeinvestigations");

            migrationBuilder.AddColumn<Guid>(
                name: "investigationsid",
                table: "schemeinvestigations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_schemeinvestigations_investigationsid",
                table: "schemeinvestigations",
                column: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_schemeinvestigations_investigations_investigationsid",
                table: "schemeinvestigations",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid");
        }
    }
}
