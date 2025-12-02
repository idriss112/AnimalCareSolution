using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class Restartdat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1b901587-dfd2-493e-82ba-8a156b602ca3", new DateTime(2025, 11, 30, 17, 32, 29, 546, DateTimeKind.Utc).AddTicks(3785), "AQAAAAIAAYagAAAAEFmlUhO2RWGbFqE1IxI1FlUPX7PWzjGQIu/LtZibV15kFOv2SMi/9R4euk6Qc3dPEg==", "fd8355d4-6fbe-4cad-a589-ca246b92051d" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 32, 29, 604, DateTimeKind.Utc).AddTicks(5672));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 32, 29, 604, DateTimeKind.Utc).AddTicks(5675));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 32, 29, 604, DateTimeKind.Utc).AddTicks(5676));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 32, 29, 604, DateTimeKind.Utc).AddTicks(5677));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 17, 32, 29, 604, DateTimeKind.Utc).AddTicks(5679));
        }
    }
}
