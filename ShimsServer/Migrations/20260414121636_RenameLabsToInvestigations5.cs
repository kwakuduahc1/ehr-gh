using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_labparameters_investigations_investigationsid1",
                table: "labparameters");

            migrationBuilder.DropForeignKey(
                name: "fk_labpayments_labrequests_investigationsrequestsid",
                table: "labpayments");

            migrationBuilder.DropForeignKey(
                name: "fk_labrequests_patientattendances_patientattendancesid",
                table: "labrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_labrequests_schemelabs_schemelabsid",
                table: "labrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_labresults_labparameters_labparametersid",
                table: "labresults");

            migrationBuilder.DropForeignKey(
                name: "fk_labresults_labpayments_investigationspaymentid",
                table: "labresults");

            migrationBuilder.DropPrimaryKey(
                name: "pk_labresults",
                table: "labresults");

            migrationBuilder.DropPrimaryKey(
                name: "pk_labrequests",
                table: "labrequests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_labpayments",
                table: "labpayments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_labparameters",
                table: "labparameters");

            migrationBuilder.RenameTable(
                name: "labresults",
                newName: "investigationsresults");

            migrationBuilder.RenameTable(
                name: "labrequests",
                newName: "investigationsrequests");

            migrationBuilder.RenameTable(
                name: "labpayments",
                newName: "investigationspayments");

            migrationBuilder.RenameTable(
                name: "labparameters",
                newName: "investigationparameters");

            migrationBuilder.RenameIndex(
                name: "IX_labresults_labparametersid",
                table: "investigationsresults",
                newName: "IX_investigationsresults_labparametersid");

            migrationBuilder.RenameIndex(
                name: "IX_labrequests_schemelabsid",
                table: "investigationsrequests",
                newName: "IX_investigationsrequests_schemelabsid");

            migrationBuilder.RenameIndex(
                name: "IX_labrequests_patientattendancesid",
                table: "investigationsrequests",
                newName: "IX_investigationsrequests_patientattendancesid");

            migrationBuilder.RenameIndex(
                name: "IX_labparameters_investigationsid1",
                table: "investigationparameters",
                newName: "IX_investigationparameters_investigationsid1");

            migrationBuilder.AddPrimaryKey(
                name: "pk_investigationsresults",
                table: "investigationsresults",
                column: "investigationspaymentid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_investigationsrequests",
                table: "investigationsrequests",
                column: "labrequestsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_investigationspayments",
                table: "investigationspayments",
                column: "investigationsrequestsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_investigationparameters",
                table: "investigationparameters",
                column: "investigationparametersid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationparameters_investigations_investigationsid1",
                table: "investigationparameters",
                column: "investigationsid1",
                principalTable: "investigations",
                principalColumn: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationspayments_investigationsrequests_investigation~",
                table: "investigationspayments",
                column: "investigationsrequestsid",
                principalTable: "investigationsrequests",
                principalColumn: "labrequestsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsrequests_schemelabs_schemelabsid",
                table: "investigationsrequests",
                column: "schemelabsid",
                principalTable: "schemelabs",
                principalColumn: "schemelabsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsresults_investigationparameters_labparameters~",
                table: "investigationsresults",
                column: "labparametersid",
                principalTable: "investigationparameters",
                principalColumn: "investigationparametersid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_investigationsresults_investigationspayments_investigations~",
                table: "investigationsresults",
                column: "investigationspaymentid",
                principalTable: "investigationspayments",
                principalColumn: "investigationsrequestsid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationparameters_investigations_investigationsid1",
                table: "investigationparameters");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationspayments_investigationsrequests_investigation~",
                table: "investigationspayments");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_patientattendances_patientattendance~",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsrequests_schemelabs_schemelabsid",
                table: "investigationsrequests");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsresults_investigationparameters_labparameters~",
                table: "investigationsresults");

            migrationBuilder.DropForeignKey(
                name: "fk_investigationsresults_investigationspayments_investigations~",
                table: "investigationsresults");

            migrationBuilder.DropPrimaryKey(
                name: "pk_investigationsresults",
                table: "investigationsresults");

            migrationBuilder.DropPrimaryKey(
                name: "pk_investigationsrequests",
                table: "investigationsrequests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_investigationspayments",
                table: "investigationspayments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_investigationparameters",
                table: "investigationparameters");

            migrationBuilder.RenameTable(
                name: "investigationsresults",
                newName: "labresults");

            migrationBuilder.RenameTable(
                name: "investigationsrequests",
                newName: "labrequests");

            migrationBuilder.RenameTable(
                name: "investigationspayments",
                newName: "labpayments");

            migrationBuilder.RenameTable(
                name: "investigationparameters",
                newName: "labparameters");

            migrationBuilder.RenameIndex(
                name: "IX_investigationsresults_labparametersid",
                table: "labresults",
                newName: "IX_labresults_labparametersid");

            migrationBuilder.RenameIndex(
                name: "IX_investigationsrequests_schemelabsid",
                table: "labrequests",
                newName: "IX_labrequests_schemelabsid");

            migrationBuilder.RenameIndex(
                name: "IX_investigationsrequests_patientattendancesid",
                table: "labrequests",
                newName: "IX_labrequests_patientattendancesid");

            migrationBuilder.RenameIndex(
                name: "IX_investigationparameters_investigationsid1",
                table: "labparameters",
                newName: "IX_labparameters_investigationsid1");

            migrationBuilder.AddPrimaryKey(
                name: "pk_labresults",
                table: "labresults",
                column: "investigationspaymentid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_labrequests",
                table: "labrequests",
                column: "labrequestsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_labpayments",
                table: "labpayments",
                column: "investigationsrequestsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_labparameters",
                table: "labparameters",
                column: "investigationparametersid");

            migrationBuilder.AddForeignKey(
                name: "fk_labparameters_investigations_investigationsid1",
                table: "labparameters",
                column: "investigationsid1",
                principalTable: "investigations",
                principalColumn: "investigationsid");

            migrationBuilder.AddForeignKey(
                name: "fk_labpayments_labrequests_investigationsrequestsid",
                table: "labpayments",
                column: "investigationsrequestsid",
                principalTable: "labrequests",
                principalColumn: "labrequestsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_labrequests_patientattendances_patientattendancesid",
                table: "labrequests",
                column: "patientattendancesid",
                principalTable: "patientattendances",
                principalColumn: "patientattendancesid");

            migrationBuilder.AddForeignKey(
                name: "fk_labrequests_schemelabs_schemelabsid",
                table: "labrequests",
                column: "schemelabsid",
                principalTable: "schemelabs",
                principalColumn: "schemelabsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_labresults_labparameters_labparametersid",
                table: "labresults",
                column: "labparametersid",
                principalTable: "labparameters",
                principalColumn: "investigationparametersid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_labresults_labpayments_investigationspaymentid",
                table: "labresults",
                column: "investigationspaymentid",
                principalTable: "labpayments",
                principalColumn: "investigationsrequestsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
