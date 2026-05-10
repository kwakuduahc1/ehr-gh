using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class LabsNavsFix3 : Migration
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
                name: "patientattendancesid1",
                table: "investigationsrequests");

            migrationBuilder.CreateIndex(
                name: "IX_investigationsrequests_patientattendancesid",
                table: "investigationsrequests",
                column: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests");

            migrationBuilder.DropIndex(
                name: "IX_investigationsrequests_patientattendancesid",
                table: "investigationsrequests");

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
