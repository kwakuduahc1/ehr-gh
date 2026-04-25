using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class DrgServStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "services",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "drugs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "services");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "drugs");
        }
    }
}
