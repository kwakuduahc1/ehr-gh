using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class GCS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "gcs",
                table: "patientconsultations",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gcs",
                table: "patientconsultations");
        }
    }
}
