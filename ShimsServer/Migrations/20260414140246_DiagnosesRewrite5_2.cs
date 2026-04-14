using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosesRewrite5_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationparameters_investigations_investigationsid1",
                table: "investigationparameters");

            migrationBuilder.DropIndex(
                name: "IX_investigationparameters_investigationsid1",
                table: "investigationparameters");

            migrationBuilder.DropColumn(
                name: "investigationsid1",
                table: "investigationparameters");

            migrationBuilder.AlterColumn<string>(
                name: "phonenumber",
                table: "patients",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "investigationsid",
            //    table: "investigationparameters",
            //    type: "uuid",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "integer");

            //migrationBuilder.CreateIndex(
            //    name: "IX_investigationparameters_investigationsid",
            //    table: "investigationparameters",
            //    column: "investigationsid");

            //migrationBuilder.AddForeignKey(
            //    name: "fk_investigationparameters_investigations_investigationsid",
            //    table: "investigationparameters",
            //    column: "investigationsid",
            //    principalTable: "investigations",
            //    principalColumn: "investigationsid",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_investigationparameters_investigations_investigationsid",
                table: "investigationparameters");

            migrationBuilder.DropIndex(
                name: "IX_investigationparameters_investigationsid",
                table: "investigationparameters");

            migrationBuilder.AlterColumn<string>(
                name: "phonenumber",
                table: "patients",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "investigationsid",
                table: "investigationparameters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "investigationsid1",
                table: "investigationparameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_investigationparameters_investigationsid1",
                table: "investigationparameters",
                column: "investigationsid1");

            migrationBuilder.AddForeignKey(
                name: "fk_investigationparameters_investigations_investigationsid1",
                table: "investigationparameters",
                column: "investigationsid1",
                principalTable: "investigations",
                principalColumn: "investigationsid");
        }
    }
}
