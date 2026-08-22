using Microsoft.EntityFrameworkCore.Migrations;
using ShimsServer.Models.ConsultingRoom;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class NeuroScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<AVPU>(
                name: "avpua",
                table: "patientconsultations",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<GCS>(
                name: "gcsa",
                table: "patientconsultations",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avpua",
                table: "patientconsultations");

            migrationBuilder.DropColumn(
                name: "gcsa",
                table: "patientconsultations");
        }
    }
}
