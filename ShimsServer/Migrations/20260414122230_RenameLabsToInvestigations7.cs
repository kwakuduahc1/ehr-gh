using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "labsgroupid",
                table: "schemeinvestigations",
                newName: "schemeinvestigationsgroupid");

            migrationBuilder.AddColumn<string>(
                name: "gdrg",
                table: "schemeinvestigations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gdrg",
                table: "schemeinvestigations");

            migrationBuilder.RenameColumn(
                name: "schemeinvestigationsgroupid",
                table: "schemeinvestigations",
                newName: "labsgroupid");
        }
    }
}
