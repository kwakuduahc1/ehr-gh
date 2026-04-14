using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class Opt04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "fk_investigationparameters_investigations_investigationsid1",
            //    table: "investigationparameters");

            //migrationBuilder.DropIndex(
            //    name: "IX_investigationparameters_investigationsid1",
            //    table: "investigationparameters");

            migrationBuilder.DropColumn(
                name: "investigationsid",
                table: "investigationparameters");

            //migrationBuilder.DropColumn(
            //    name: "investigationsid1",
            //    table: "investigationparameters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.AddColumn<int>(
            //        name: "investigationsid",
            //        table: "investigationparameters",
            //        type: "integer",
            //        nullable: false,
            //        defaultValue: 0);

            //    migrationBuilder.AddColumn<Guid>(
            //        name: "investigationsid1",
            //        table: "investigationparameters",
            //        type: "uuid",
            //        nullable: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_investigationparameters_investigationsid1",
            //        table: "investigationparameters",
            //        column: "investigationsid1");

            //    migrationBuilder.AddForeignKey(
            //        name: "fk_investigationparameters_investigations_investigationsid1",
            //        table: "investigationparameters",
            //        column: "investigationsid1",
            //        principalTable: "investigations",
            //        principalColumn: "investigationsid");
        }
    }
}
