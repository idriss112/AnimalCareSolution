
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class Restartdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6170938-3b8a-4d60-b83e-67f116e4585a", new DateTime(2025, 11, 30, 4, 44, 5, 132, DateTimeKind.Utc).AddTicks(3038), "AQAAAAIAAYagAAAAEEORDnVd9wjd02kSxQ7ypvGtho11Waebz0OF4beyZrtvxu3INWcAjJboIFd7YRrGdw==", "72e807ca-9ebc-49c8-88d0-f5d25ddc73b7" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 4, 44, 5, 192, DateTimeKind.Utc).AddTicks(4041));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 4, 44, 5, 192, DateTimeKind.Utc).AddTicks(4048));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 4, 44, 5, 192, DateTimeKind.Utc).AddTicks(4049));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 4, 44, 5, 192, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 4, 44, 5, 192, DateTimeKind.Utc).AddTicks(4151));
        }
    }
}
