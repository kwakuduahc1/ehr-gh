using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavsFix4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patientattendances_patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_patientoutcomes",
                table: "patientoutcomes");

            migrationBuilder.DropIndex(
                name: "IX_drugsrequests_patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "servicerequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "drugsrequests");

            migrationBuilder.AddColumn<Guid>(
                name: "patientoutcomesid",
                table: "patientoutcomes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.AddPrimaryKey(
                name: "pk_patientoutcomes",
                table: "patientoutcomes",
                column: "patientoutcomesid");

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientattendancesid",
                table: "servicerequests",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_patientoutcomes_patientsattendancesid",
                table: "patientoutcomes",
                column: "patientsattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_patientattendancesid",
                table: "drugsrequests",
                column: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_patientattendances_patientattendancesid",
                table: "drugsrequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid",
                table: "servicerequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_patientattendances_patientattendancesid",
                table: "drugsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_servicerequests_patientattendances_patientattendancesid",
                table: "servicerequests");

            migrationBuilder.DropIndex(
                name: "IX_servicerequests_patientattendancesid",
                table: "servicerequests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_patientoutcomes",
                table: "patientoutcomes");

            migrationBuilder.DropIndex(
                name: "IX_patientoutcomes_patientsattendancesid",
                table: "patientoutcomes");

            migrationBuilder.DropIndex(
                name: "IX_drugsrequests_patientattendancesid",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "patientoutcomesid",
                table: "patientoutcomes");

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "servicerequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "drugsrequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_patientoutcomes",
                table: "patientoutcomes",
                column: "patientsattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientattendancesid1",
                table: "servicerequests",
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
                name: "fk_servicerequests_patientattendances_patientattendancesid1",
                table: "servicerequests",
                column: "patientattendancesid1",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }
    }
}
