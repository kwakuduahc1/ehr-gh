using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class ConsultPtAttFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientconsultations_patientattendances_patientattendancesid",
                table: "patientconsultations");

            migrationBuilder.DropColumn(
                name: "patientsattendancesid",
                table: "patientconsultations");

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "patientconsultations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_patientconsultations_patientattendances_patientattendancesid",
                table: "patientconsultations",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientconsultations_patientattendances_patientattendancesid",
                table: "patientconsultations");

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "patientconsultations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "patientsattendancesid",
                table: "patientconsultations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "fk_patientconsultations_patientattendances_patientattendancesid",
                table: "patientconsultations",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }
    }
}
