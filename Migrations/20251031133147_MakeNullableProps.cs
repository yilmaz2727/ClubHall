using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OgrenciKulupSistemi.Migrations
{
    /// <inheritdoc />
    public partial class MakeNullableProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EventPhotoUrl",
                table: "Events",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "LogoImageUrl",
                table: "Clubs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CoverPhotoUrl",
                table: "Clubs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "CoverPhotoUrl", "Description", "LogoImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "", "Kampüsün ritmini biz belirleriz. Müzik ve eğlence burada.", "", "SAÜ Rock Topluluğu" },
                    { 2, "", "Yazılım ve teknoloji meraklılarının buluşma noktası.", "", "Saü Bilgisayar Topluluğu" },
                    { 3, "", "Erasmus Student Network of ESN Sakarya University The Official Page of ESN SAKARYA", "", "SAÜ ESN" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "ClubId", "Description", "EventDate", "EventPhotoUrl", "Location", "Title" },
                values: new object[,]
                {
                    { 1, 2, "Takımını oluştur, becerilerini göster!", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Bilgisayar ve Bilişim Bilimleri Fakültesi 1109", "ASP.NET Core Hackathon" },
                    { 2, 1, "Tiyatro Topluluğu’nun düzenlemiş olduğu 1. Tiyatro Günleri’nde biz de SaüRock olarak sahnedeyiz! ", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Turgut Özal Kültür ve Kongre Merkezi", "SAÜ Rock The Band Sahnede" },
                    { 3, 3, "Practice Engilsh and Meet with new people!", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Saü Taş Kafe", "Spekaing CLub First Meeting" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "EventPhotoUrl",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoImageUrl",
                table: "Clubs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverPhotoUrl",
                table: "Clubs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
