using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class VitalsNulling2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vitals_patientattendances_patientattendancesid",
                table: "vitals");

            migrationBuilder.DropColumn(
                name: "patientsattendancesid",
                table: "vitals");

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "vitals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_vitals_patientattendances_patientattendancesid",
                table: "vitals",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vitals_patientattendances_patientattendancesid",
                table: "vitals");

            migrationBuilder.AlterColumn<Guid>(
                name: "patientattendancesid",
                table: "vitals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "patientsattendancesid",
                table: "vitals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "fk_vitals_patientattendances_patientattendancesid",
                table: "vitals",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }
    }
}
