using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations
{
    /// <inheritdoc />
    public partial class WnioskiFormv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "WnioskiForms",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "IDD",
                table: "WnioskiForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IDS",
                table: "WnioskiForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NazwaPlikuWniosku",
                table: "WnioskiForms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDD",
                table: "WnioskiForms");

            migrationBuilder.DropColumn(
                name: "IDS",
                table: "WnioskiForms");

            migrationBuilder.DropColumn(
                name: "NazwaPlikuWniosku",
                table: "WnioskiForms");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "WnioskiForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
