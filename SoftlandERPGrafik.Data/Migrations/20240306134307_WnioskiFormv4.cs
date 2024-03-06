using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations
{
    /// <inheritdoc />
    public partial class WnioskiFormv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NazwaPlikuWniosku",
                table: "WnioskiForms");

            migrationBuilder.AddColumn<int>(
                name: "IloscDni",
                table: "WnioskiForms",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IloscDni",
                table: "WnioskiForms");

            migrationBuilder.AddColumn<string>(
                name: "NazwaPlikuWniosku",
                table: "WnioskiForms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
