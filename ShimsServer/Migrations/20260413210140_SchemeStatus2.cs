using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class SchemeStatus2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "servicegroup",
                table: "services",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "icdcode",
                table: "services",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "icdversion",
                table: "services",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "10");

            migrationBuilder.AddColumn<short[]>(
                name: "allowedtiers",
                table: "schemeservices",
                type: "smallint[]",
                nullable: false,
                defaultValue: new short[0]);

            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                table: "schemeservices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "icdcode",
                table: "services");

            migrationBuilder.DropColumn(
                name: "icdversion",
                table: "services");

            migrationBuilder.DropColumn(
                name: "allowedtiers",
                table: "schemeservices");

            migrationBuilder.DropColumn(
                name: "isactive",
                table: "schemeservices");

            migrationBuilder.AlterColumn<string>(
                name: "servicegroup",
                table: "services",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
