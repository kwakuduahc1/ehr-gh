using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patients_patientsid",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patients_patientsid",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientsid",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_investigationsrequests_patientattendancesid",
                table: "investigationsrequests");

            migrationBuilder.DropIndex(
                name: "IX_drugsrequests_patientsid",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "patientsattendancesid",
                table: "investigationsrequests");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "servicerequests",
                newName: "patientattendancesid");

            migrationBuilder.RenameColumn(
                name: "patientsid",
                table: "drugsrequests",
                newName: "patientattendancesid");

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "servicerequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "investigationsrequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "investigationsrequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "drugsrequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientattendancesid1",
                table: "servicerequests",
                column: "patientattendancesid1");

            migrationBuilder.CreateIndex(
                name: "IX_investigationsrequests_patientattendancesid1",
                table: "investigationsrequests",
                column: "patientattendancesid1");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_patientattendancesid1",
                table: "drugsrequests",
                column: "patientattendancesid1");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_patientattendances_patientattendancesid1",
                table: "drugsrequests",
                column: "patientattendancesid1",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests",
                column: "patientattendancesid1",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid1",
                table: "servicerequests",
                column: "patientattendancesid1",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patientattendances_patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_investigationsrequests_patientattendancesid1",
                table: "investigationsrequests");

            migrationBuilder.DropIndex(
                name: "IX_drugsrequests_patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "investigationsrequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.RenameColumn(
                name: "patientattendancesid",
                table: "servicerequests",
                newName: "patientsid");

            migrationBuilder.RenameColumn(
                name: "patientattendancesid",
                table: "drugsrequests",
                newName: "patientsid");

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "investigationsrequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "patientsattendancesid",
                table: "investigationsrequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientsid",
                table: "servicerequests",
                column: "patientsid");

            migrationBuilder.CreateIndex(
                name: "IX_investigationsrequests_patientattendancesid",
                table: "investigationsrequests",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_patientsid",
                table: "drugsrequests",
                column: "patientsid");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_patients_patientsid",
                table: "drugsrequests",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_servicerequests_patients_patientsid",
                table: "servicerequests",
                column: "patientsid",
                principalTable: "patients",
                principalColumn: "patientsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
