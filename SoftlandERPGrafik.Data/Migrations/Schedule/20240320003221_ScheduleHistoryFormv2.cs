using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations.Schedule
{
    /// <inheritdoc />
    public partial class ScheduleHistoryFormv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "After",
                table: "ScheduleHistoryForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Before",
                table: "ScheduleHistoryForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Column",
                table: "ScheduleHistoryForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "scheduleId",
                table: "ScheduleHistoryForms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "After",
                table: "ScheduleHistoryForms");

            migrationBuilder.DropColumn(
                name: "Before",
                table: "ScheduleHistoryForms");

            migrationBuilder.DropColumn(
                name: "Column",
                table: "ScheduleHistoryForms");

            migrationBuilder.DropColumn(
                name: "scheduleId",
                table: "ScheduleHistoryForms");
        }
    }
}
