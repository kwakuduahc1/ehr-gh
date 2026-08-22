using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class NeuroScoresFom1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avpu",
                table: "patientconsultations");

            migrationBuilder.DropColumn(
                name: "gcs",
                table: "patientconsultations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "avpu",
                table: "patientconsultations",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "gcs",
                table: "patientconsultations",
                type: "smallint",
                nullable: true);
        }
    }
}
