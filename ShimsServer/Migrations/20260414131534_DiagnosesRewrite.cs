using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientdiagnoses_diagnoses_diagnosesdiagnosisid",
                table: "patientdiagnoses");

            migrationBuilder.DropIndex(
                name: "IX_diagnoses_icd_gdrg_snomed",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "gdrg",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "icd",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "snomed",
                table: "diagnoses");

            migrationBuilder.RenameColumn(
                name: "diagnosesdiagnosisid",
                table: "patientdiagnoses",
                newName: "diagnosesid");

            migrationBuilder.RenameIndex(
                name: "IX_patientdiagnoses_diagnosesdiagnosisid",
                table: "patientdiagnoses",
                newName: "IX_patientdiagnoses_diagnosesid");

            migrationBuilder.RenameColumn(
                name: "diagnosisid",
                table: "diagnoses",
                newName: "diagnosesid");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "diagnoses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "diagnoses",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "diagnosisname",
                table: "diagnoses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                table: "diagnoses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "subcategory",
                table: "diagnoses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "schemediagnoses",
                columns: table => new
                {
                    schemediagnosesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    snomed = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    diagnosesid = table.Column<Guid>(type: "uuid", nullable: false),
                    gdrg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    dateset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemediagnoses", x => x.schemediagnosesid);
                    table.ForeignKey(
                        name: "fk_schemediagnoses_diagnoses_diagnosesid",
                        column: x => x.diagnosesid,
                        principalTable: "diagnoses",
                        principalColumn: "diagnosesid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_schemediagnoses_schemes_schemesid",
                        column: x => x.schemesid,
                        principalTable: "schemes",
                        principalColumn: "schemesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_diagnoses_diagnosisname_category_subcategory",
                table: "diagnoses",
                columns: ["diagnosisname", "category", "subcategory"]);

            migrationBuilder.CreateIndex(
                name: "IX_schemediagnoses_diagnosesid",
                table: "schemediagnoses",
                column: "diagnosesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemediagnoses_schemesid",
                table: "schemediagnoses",
                column: "schemesid");

            migrationBuilder.AddForeignKey(
                name: "fk_patientdiagnoses_diagnoses_diagnosesid",
                table: "patientdiagnoses",
                column: "diagnosesid",
                principalTable: "diagnoses",
                principalColumn: "diagnosesid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patientdiagnoses_diagnoses_diagnosesid",
                table: "patientdiagnoses");

            migrationBuilder.DropTable(
                name: "schemediagnoses");

            migrationBuilder.DropIndex(
                name: "IX_diagnoses_diagnosisname_category_subcategory",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "category",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "description",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "diagnosisname",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "isactive",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "subcategory",
                table: "diagnoses");

            migrationBuilder.RenameColumn(
                name: "diagnosesid",
                table: "patientdiagnoses",
                newName: "diagnosesdiagnosisid");

            migrationBuilder.RenameIndex(
                name: "IX_patientdiagnoses_diagnosesid",
                table: "patientdiagnoses",
                newName: "IX_patientdiagnoses_diagnosesdiagnosisid");

            migrationBuilder.RenameColumn(
                name: "diagnosesid",
                table: "diagnoses",
                newName: "diagnosisid");

            migrationBuilder.AddColumn<string>(
                name: "gdrg",
                table: "diagnoses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "icd",
                table: "diagnoses",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "snomed",
                table: "diagnoses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_diagnoses_icd_gdrg_snomed",
                table: "diagnoses",
                columns: new[] { "icd", "gdrg", "snomed" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_patientdiagnoses_diagnoses_diagnosesdiagnosisid",
                table: "patientdiagnoses",
                column: "diagnosesdiagnosisid",
                principalTable: "diagnoses",
                principalColumn: "diagnosisid");
        }
    }
}
