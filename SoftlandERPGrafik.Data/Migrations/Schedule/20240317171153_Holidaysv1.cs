using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations.Schedule
{
    /// <inheritdoc />
    public partial class Holidaysv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HolidaysData",
                columns: table => new
                {
                    Data = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rodzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rok = table.Column<int>(type: "int", nullable: false),
                    Mies = table.Column<int>(type: "int", nullable: false),
                    Dzien = table.Column<int>(type: "int", nullable: false),
                    DzienTyg = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidaysData", x => x.Data);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidaysData");
        }
    }
}
