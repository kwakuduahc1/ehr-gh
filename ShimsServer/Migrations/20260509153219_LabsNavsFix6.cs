using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavsFix6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "labrequestsid",
                table: "investigationsrequests",
                newName: "investigationsrequestsid");

            migrationBuilder.RenameColumn(
                name: "investigationsids",
                table: "investigations",
                newName: "investigationsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "investigationsrequestsid",
                table: "investigationsrequests",
                newName: "labrequestsid");

            migrationBuilder.RenameColumn(
                name: "investigationsid",
                table: "investigations",
                newName: "investigationsids");
        }
    }
}
