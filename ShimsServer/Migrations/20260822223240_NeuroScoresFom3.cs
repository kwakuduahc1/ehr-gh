using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class NeuroScoresFom3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "gcsa",
                table: "patientconsultations",
                newName: "gcs");

            migrationBuilder.RenameColumn(
                name: "avpua",
                table: "patientconsultations",
                newName: "avpu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "gcs",
                table: "patientconsultations",
                newName: "gcsa");

            migrationBuilder.RenameColumn(
                name: "avpu",
                table: "patientconsultations",
                newName: "avpua");
        }
    }
}
