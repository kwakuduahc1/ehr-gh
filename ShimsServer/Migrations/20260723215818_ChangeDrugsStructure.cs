using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDrugsStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_schemedrugs_schemedrugsid",
                table: "drugsrequests");

            migrationBuilder.DropTable(
                name: "dispensings");

            migrationBuilder.DropTable(
                name: "drugpayments");

            migrationBuilder.DropTable(
                name: "dispensingcalculations");

            migrationBuilder.DropColumn(
                name: "days",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "frequency",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "quantityrequested",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "username",
                table: "drugsrequests");

            migrationBuilder.RenameColumn(
                name: "daterequested",
                table: "drugsrequests",
                newName: "requestdate");

            migrationBuilder.AlterColumn<Guid>(
                name: "schemedrugsid",
                table: "drugsrequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "datedispensed",
                table: "drugsrequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "datepaid",
                table: "drugsrequests",
                type: "timestamp with time zone",
                nullable: true
                );

            migrationBuilder.AddColumn<string>(
                name: "dispensinguser",
                table: "drugsrequests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paymentreceipt",
                table: "drugsrequests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "paymentuser",
                table: "drugsrequests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "physician",
                table: "drugsrequests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "drugsrequestdetails",
                columns: table => new
                {
                    drugsrequestdetailsid = table.Column<Guid>(type: "uuid", nullable: false),
                    drugsrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemedrugsid = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<byte>(type: "smallint", nullable: false),
                    days = table.Column<byte>(type: "smallint", nullable: false),
                    isquantityset = table.Column<bool>(type: "boolean", nullable: false),
                    quantityrequested = table.Column<byte>(type: "smallint", nullable: true),
                    daterequested = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdispensed = table.Column<bool>(type: "boolean", nullable: false),
                    quantitydispensed = table.Column<byte>(type: "smallint", nullable: true),
                    datedispensed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugsrequestdetails", x => x.drugsrequestdetailsid);
                    table.ForeignKey(
                        name: "fk_drugsrequestdetails_drugsrequests_drugsrequestsid",
                        column: x => x.drugsrequestsid,
                        principalTable: "drugsrequests",
                        principalColumn: "drugsrequestsid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_drugsrequestdetails_schemedrugs_schemedrugsid",
                        column: x => x.schemedrugsid,
                        principalTable: "schemedrugs",
                        principalColumn: "schemedrugsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequestdetails_drugsrequestsid",
                table: "drugsrequestdetails",
                column: "drugsrequestsid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequestdetails_schemedrugsid",
                table: "drugsrequestdetails",
                column: "schemedrugsid");

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_schemedrugs_schemedrugsid",
                table: "drugsrequests",
                column: "schemedrugsid",
                principalTable: "schemedrugs",
                principalColumn: "schemedrugsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drugsrequests_schemedrugs_schemedrugsid",
                table: "drugsrequests");

            migrationBuilder.DropTable(
                name: "drugsrequestdetails");

            migrationBuilder.DropColumn(
                name: "datedispensed",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "datepaid",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "dispensinguser",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "paymentreceipt",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "paymentuser",
                table: "drugsrequests");

            migrationBuilder.DropColumn(
                name: "physician",
                table: "drugsrequests");

            migrationBuilder.RenameColumn(
                name: "requestdate",
                table: "drugsrequests",
                newName: "daterequested");

            migrationBuilder.AlterColumn<Guid>(
                name: "schemedrugsid",
                table: "drugsrequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "days",
                table: "drugsrequests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "frequency",
                table: "drugsrequests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "quantityrequested",
                table: "drugsrequests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "drugsrequests",
                type: "character varying(75)",
                maxLength: 75,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "dispensingcalculations",
                columns: table => new
                {
                    drugsrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    datedone = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    quantity = table.Column<byte>(type: "smallint", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispensingcalculations", x => x.drugsrequestsid);
                    table.ForeignKey(
                        name: "fk_dispensingcalculations_drugsrequests_drugsrequestsid",
                        column: x => x.drugsrequestsid,
                        principalTable: "drugsrequests",
                        principalColumn: "drugsrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drugpayments",
                columns: table => new
                {
                    dispensingcaculationsid = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    datepaid = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    paymenttypesid = table.Column<Guid>(type: "uuid", nullable: true),
                    quantitypaid = table.Column<byte>(type: "smallint", nullable: false),
                    receipt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugpayments", x => x.dispensingcaculationsid);
                    table.ForeignKey(
                        name: "fk_drugpayments_dispensingcalculations_dispensingcaculationsid",
                        column: x => x.dispensingcaculationsid,
                        principalTable: "dispensingcalculations",
                        principalColumn: "drugsrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dispensings",
                columns: table => new
                {
                    drugpaymentsid = table.Column<Guid>(type: "uuid", nullable: false),
                    datedispensed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantitydispensed = table.Column<byte>(type: "smallint", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispensings", x => x.drugpaymentsid);
                    table.ForeignKey(
                        name: "fk_dispensings_drugpayments_drugpaymentsid",
                        column: x => x.drugpaymentsid,
                        principalTable: "drugpayments",
                        principalColumn: "dispensingcaculationsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_drugsrequests_schemedrugs_schemedrugsid",
                table: "drugsrequests",
                column: "schemedrugsid",
                principalTable: "schemedrugs",
                principalColumn: "schemedrugsid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
