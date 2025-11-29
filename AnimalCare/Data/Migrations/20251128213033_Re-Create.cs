using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e0fa449c-1e40-465a-8fe0-20cd2cfff4ab", new DateTime(2025, 11, 28, 21, 30, 32, 349, DateTimeKind.Utc).AddTicks(7899), "AQAAAAIAAYagAAAAELxcyRGc0NPztQsYOvUg2oMxuHE9kxujrNSySAHrN8pAxhtkWELarcsLatZVmPEa3Q==", "84d5d6fc-cae2-4917-bacc-ef353a14949f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f9163576-d3e1-4b96-9b6d-8681ce7a9036", new DateTime(2025, 11, 28, 21, 12, 33, 941, DateTimeKind.Utc).AddTicks(4668), "AQAAAAIAAYagAAAAEKr9ubLTtuyFUMzmDLvhI1qpyesJpSsbkR5bh1CITJ7/YnLEcM+xCRlqvZlrPqOCJw==", "b0a23477-c8cf-45c2-a08d-ba2b3d62e5e5" });
        }
    }
}
