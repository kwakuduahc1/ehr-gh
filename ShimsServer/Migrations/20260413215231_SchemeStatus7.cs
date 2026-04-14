using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class SchemeStatus7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "allowedtiers",
                table: "schemeservices",
                type: "varchar(3)[]",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "allowedtiers",
                table: "schemeservices",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "varchar(3)[]",
                oldMaxLength: 50);
        }
    }
}
