using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavsFix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests");

            migrationBuilder.DropIndex(
                name: "IX_investigationsrequests_patientattendancesid1",
                table: "investigationsrequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid",
                table: "investigationsrequests");

            migrationBuilder.DropColumn(
                name: "patientattendancesid1",
                table: "investigationsrequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid",
                table: "investigationsrequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "patientattendancesid1",
                table: "investigationsrequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_investigationsrequests_patientattendancesid1",
                table: "investigationsrequests",
                column: "patientattendancesid1");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests",
                column: "patientattendancesid1",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");
        }
    }
}
