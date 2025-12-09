using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c471be5-91a0-45b9-ab7f-a65ca10bcbd9", new DateTime(2025, 12, 9, 3, 18, 27, 635, DateTimeKind.Utc).AddTicks(1796), "AQAAAAIAAYagAAAAEGf4yToP8SOFJxL17Wh3mssmKNUW/bX7lPDHByRauq+GptCrh9agwGVqlFcQehFOhA==", "49162045-303a-4a82-a1dd-78da09beb134" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 18, 27, 693, DateTimeKind.Utc).AddTicks(4339));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 18, 27, 693, DateTimeKind.Utc).AddTicks(4344));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 18, 27, 693, DateTimeKind.Utc).AddTicks(4345));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 18, 27, 693, DateTimeKind.Utc).AddTicks(4346));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 18, 27, 693, DateTimeKind.Utc).AddTicks(4347));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0436b4b8-117a-4654-91c1-94b4a83829b9", new DateTime(2025, 11, 30, 17, 34, 5, 292, DateTimeKind.Utc).AddTicks(6055), "AQAAAAIAAYagAAAAEGb+CRYEvJHNOCF2brANqyMZJU5/+A+40xhAReDH84vEQMQtrKxHhXT6IpryGMhZYA==", "4787ff16-fbc3-4140-83df-1a27f81a6b86" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 34, 5, 348, DateTimeKind.Utc).AddTicks(8561));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 34, 5, 348, DateTimeKind.Utc).AddTicks(8569));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 34, 5, 348, DateTimeKind.Utc).AddTicks(8570));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 34, 5, 348, DateTimeKind.Utc).AddTicks(8571));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 34, 5, 348, DateTimeKind.Utc).AddTicks(8572));
        }
    }
}
