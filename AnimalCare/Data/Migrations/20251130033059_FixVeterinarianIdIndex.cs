using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixVeterinarianIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc31d6b0-6dad-4d0a-95e9-0e28605924c1", new DateTime(2025, 11, 30, 3, 30, 58, 652, DateTimeKind.Utc).AddTicks(3107), "AQAAAAIAAYagAAAAEFwYmpJ1V6NPXm8CIam0Wk9AwKxDLpMWMBXU1QeI90HBSb7OEwIxYLZVq2HpOQzPfQ==", "5094b544-e034-4a61-9a91-5f85b9c70555" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 3, 30, 58, 720, DateTimeKind.Utc).AddTicks(2166));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 3, 30, 58, 720, DateTimeKind.Utc).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 3, 30, 58, 720, DateTimeKind.Utc).AddTicks(2174));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 3, 30, 58, 720, DateTimeKind.Utc).AddTicks(2175));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 3, 30, 58, 720, DateTimeKind.Utc).AddTicks(2176));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4854116e-d644-4216-955b-4f56b9d1006d", new DateTime(2025, 11, 29, 18, 0, 45, 937, DateTimeKind.Utc).AddTicks(9716), "AQAAAAIAAYagAAAAEAeLqa4jRXH0BEN8kGEZR/BM5fnFX0oRsbJFpAmlJtBXmx+paeKzoG3nLYIuIAu8zg==", "22a331d8-4e68-4c6d-9e33-3391ea7c756a" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 18, 0, 45, 996, DateTimeKind.Utc).AddTicks(4520));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 18, 0, 45, 996, DateTimeKind.Utc).AddTicks(4524));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 18, 0, 45, 996, DateTimeKind.Utc).AddTicks(4525));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 18, 0, 45, 996, DateTimeKind.Utc).AddTicks(4527));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 18, 0, 45, 996, DateTimeKind.Utc).AddTicks(4528));
        }
    }
}
