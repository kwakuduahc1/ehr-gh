using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class AttPats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientattendances_patientschemes_patientschemesid",
                table: "patientattendances");

            migrationBuilder.RenameColumn(
                name: "patientschemesid",
                table: "patientattendances",
                newName: "patientsid");

            migrationBuilder.RenameIndex(
                name: "IX_patientattendances_patientschemesid",
                table: "patientattendances",
                newName: "IX_patientattendances_patientsid");

            migrationBuilder.CreateIndex(
                name: "IX_patientschemes_patientsid",
                table: "patientschemes",
                column: "patientsid");

            migrationBuilder.AddForeignKey(
                name: "fk_patientattendances_patients_patientsid",
                table: "patientattendances",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_patientschemes_patients_patientsid",
                table: "patientschemes",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientattendances_patients_patientsid",
                table: "patientattendances");

            migrationBuilder.DropForeignKey(
                name: "fk_patientschemes_patients_patientsid",
                table: "patientschemes");

            migrationBuilder.DropIndex(
                name: "IX_patientschemes_patientsid",
                table: "patientschemes");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "patientattendances",
                newName: "patientschemesid");

            migrationBuilder.RenameIndex(
                name: "IX_patientattendances_patientsid",
                table: "patientattendances",
                newName: "IX_patientattendances_patientschemesid");

            migrationBuilder.AddForeignKey(
                name: "fk_patientattendances_patientschemes_patientschemesid",
                table: "patientattendances",
                column: "patientschemesid",
                principalTable: "patientschemes",
                principalColumn: "patientschemesid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
