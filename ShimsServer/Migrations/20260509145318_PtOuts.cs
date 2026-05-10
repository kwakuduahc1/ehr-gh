using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class PtOuts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patientattendances_patientsattendancesid",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientattendancesid",
                table: "servicerequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid",
                table: "servicerequests");

            migrationBuilder.RenameColumn(
                name: "patientsattendancesid",
                table: "servicerequests",
                newName: "patientsid");

            migrationBuilder.RenameColumn(
                name: "patientsattendancesid",
                table: "servicerenderings",
                newName: "patientsid");

            migrationBuilder.RenameColumn(
                name: "patientsattendancesid",
                table: "servicepayments",
                newName: "patientsid");

            migrationBuilder.RenameColumn(
                name: "patientsattendancesid",
                table: "drugsrequests",
                newName: "patientsid");

            migrationBuilder.RenameIndex(
                name: "IX_drugsrequests_patientsattendancesid",
                table: "drugsrequests",
                newName: "IX_drugsrequests_patientsid");

            migrationBuilder.AlterColumn<string>(
                name: "signandsymptoms",
                table: "patientsignsandsymptoms",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "patientid",
                table: "patientoutcomes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "patientoutcomes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientsid",
                table: "servicerequests",
                column: "patientsid");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_patients_patientsid",
                table: "drugsrequests",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_servicerequests_patients_patientsid",
                table: "servicerequests",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patients_patientsid",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patients_patientsid",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientsid",
                table: "servicerequests");

            migrationBuilder.DropColumn(
                name: "patientid",
                table: "patientoutcomes");

            migrationBuilder.DropColumn(
                name: "username",
                table: "patientoutcomes");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "servicerequests",
                newName: "patientsattendancesid");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "servicerenderings",
                newName: "patientsattendancesid");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "servicepayments",
                newName: "patientsattendancesid");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "drugsrequests",
                newName: "patientsattendancesid");

            migrationBuilder.RenameIndex(
                name: "IX_drugsrequests_patientsid",
                table: "drugsrequests",
                newName: "IX_drugsrequests_patientsattendancesid");

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid",
                table: "servicerequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "signandsymptoms",
                table: "patientsignsandsymptoms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(400)",
                oldMaxLength: 400);

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientattendancesid",
                table: "servicerequests",
                column: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_patientattendances_patientsattendancesid",
                table: "drugsrequests",
                column: "patientsattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid",
                table: "servicerequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }
    }
}
