using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciKulupSistemi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHasDataEventDate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartDate",
                value: new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartDate",
                value: new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
