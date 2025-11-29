using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fixing_VetSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8d40216-b331-4cf8-a9b1-05668e374421", new DateTime(2025, 11, 28, 21, 36, 1, 860, DateTimeKind.Utc).AddTicks(4431), "AQAAAAIAAYagAAAAEB353YagQRZzG/QfLA56yUpG/s5iVy//AtObdEveqFkiZkkjhJiJgujVrVVh2fcHUQ==", "a95e9a75-b7de-4941-8a5b-3cd7b4f9b015" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e0fa449c-1e40-465a-8fe0-20cd2cfff4ab", new DateTime(2025, 11, 28, 21, 30, 32, 349, DateTimeKind.Utc).AddTicks(7899), "AQAAAAIAAYagAAAAELxcyRGc0NPztQsYOvUg2oMxuHE9kxujrNSySAHrN8pAxhtkWELarcsLatZVmPEa3Q==", "84d5d6fc-cae2-4917-bacc-ef353a14949f" });
        }
    }
}
