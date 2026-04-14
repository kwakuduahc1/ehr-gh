using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabsToInvestigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_labparameters_labgroups_labgroupsid1",
                table: "labparameters");

            migrationBuilder.DropForeignKey(
                name: "fk_labpayments_labrequests_labrequestsid",
                table: "labpayments");

            migrationBuilder.DropForeignKey(
                name: "fk_labresults_labpayments_labpaymentid",
                table: "labresults");

            migrationBuilder.DropForeignKey(
                name: "fk_schemelabs_labgroups_labgroupsid",
                table: "schemelabs");

            migrationBuilder.DropTable(
                name: "labgroups");

            migrationBuilder.RenameColumn(
                name: "labgroupsid",
                table: "schemelabs",
                newName: "investigationsid");

            migrationBuilder.RenameIndex(
                name: "IX_schemelabs_labgroupsid",
                table: "schemelabs",
                newName: "IX_schemelabs_investigationsid");

            migrationBuilder.RenameColumn(
                name: "labpaymentid",
                table: "labresults",
                newName: "investigationspaymentid");

            migrationBuilder.RenameColumn(
                name: "labrequestsid",
                table: "labpayments",
                newName: "investigationsrequestsid");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "labparameters",
                newName: "parameterorder");

            migrationBuilder.RenameColumn(
                name: "labparameter",
                table: "labparameters",
                newName: "investigationparameter");

            migrationBuilder.RenameColumn(
                name: "labgroupsid1",
                table: "labparameters",
                newName: "investigationsid1");

            migrationBuilder.RenameColumn(
                name: "labgroupsid",
                table: "labparameters",
                newName: "investigationsid");

            migrationBuilder.RenameColumn(
                name: "labparametersid",
                table: "labparameters",
                newName: "investigationparametersid");

            migrationBuilder.RenameIndex(
                name: "IX_labparameters_labgroupsid1",
                table: "labparameters",
                newName: "IX_labparameters_investigationsid1");

            migrationBuilder.CreateTable(
                name: "investigations",
                columns: table => new
                {
                    investigationsid = table.Column<Guid>(type: "uuid", nullable: false),
                    investigation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    investigationtype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    levels = table.Column<string[]>(type: "text[]", nullable: false),
                    labdescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_investigations", x => x.investigationsid);
                });

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
                name: "fk_labresults_labpayments_investigationspaymentid",
                table: "labresults",
                column: "investigationspaymentid",
                principalTable: "labpayments",
                principalColumn: "investigationsrequestsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemelabs_investigations_investigationsid",
                table: "schemelabs",
                column: "investigationsid",
                principalTable: "investigations",
                principalColumn: "investigationsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_labparameters_investigations_investigationsid1",
                table: "labparameters");

            migrationBuilder.DropForeignKey(
                name: "fk_labpayments_labrequests_investigationsrequestsid",
                table: "labpayments");

            migrationBuilder.DropForeignKey(
                name: "fk_labresults_labpayments_investigationspaymentid",
                table: "labresults");

            migrationBuilder.DropForeignKey(
                name: "fk_schemelabs_investigations_investigationsid",
                table: "schemelabs");

            migrationBuilder.DropTable(
                name: "investigations");

            migrationBuilder.RenameColumn(
                name: "investigationsid",
                table: "schemelabs",
                newName: "labgroupsid");

            migrationBuilder.RenameIndex(
                name: "IX_schemelabs_investigationsid",
                table: "schemelabs",
                newName: "IX_schemelabs_labgroupsid");

            migrationBuilder.RenameColumn(
                name: "investigationspaymentid",
                table: "labresults",
                newName: "labpaymentid");

            migrationBuilder.RenameColumn(
                name: "investigationsrequestsid",
                table: "labpayments",
                newName: "labrequestsid");

            migrationBuilder.RenameColumn(
                name: "parameterorder",
                table: "labparameters",
                newName: "order");

            migrationBuilder.RenameColumn(
                name: "investigationsid1",
                table: "labparameters",
                newName: "labgroupsid1");

            migrationBuilder.RenameColumn(
                name: "investigationsid",
                table: "labparameters",
                newName: "labgroupsid");

            migrationBuilder.RenameColumn(
                name: "investigationparameter",
                table: "labparameters",
                newName: "labparameter");

            migrationBuilder.RenameColumn(
                name: "investigationparametersid",
                table: "labparameters",
                newName: "labparametersid");

            migrationBuilder.RenameIndex(
                name: "IX_labparameters_investigationsid1",
                table: "labparameters",
                newName: "IX_labparameters_labgroupsid1");

            migrationBuilder.CreateTable(
                name: "labgroups",
                columns: table => new
                {
                    labgroupsid = table.Column<Guid>(type: "uuid", nullable: false),
                    labdescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    labgroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labgroups", x => x.labgroupsid);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_labparameters_labgroups_labgroupsid1",
                table: "labparameters",
                column: "labgroupsid1",
                principalTable: "labgroups",
                principalColumn: "labgroupsid");

            migrationBuilder.AddForeignKey(
                name: "fk_labpayments_labrequests_labrequestsid",
                table: "labpayments",
                column: "labrequestsid",
                principalTable: "labrequests",
                principalColumn: "labrequestsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_labresults_labpayments_labpaymentid",
                table: "labresults",
                column: "labpaymentid",
                principalTable: "labpayments",
                principalColumn: "labrequestsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_schemelabs_labgroups_labgroupsid",
                table: "schemelabs",
                column: "labgroupsid",
                principalTable: "labgroups",
                principalColumn: "labgroupsid");
        }
    }
}
