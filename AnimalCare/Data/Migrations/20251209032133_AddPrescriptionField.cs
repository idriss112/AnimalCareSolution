using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prescription",
                table: "Appointments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f373f52-df83-4823-8a36-4ba76d75809f", new DateTime(2025, 12, 9, 3, 21, 32, 268, DateTimeKind.Utc).AddTicks(6201), "AQAAAAIAAYagAAAAEAFo5T1U9oiK7+3qey9fKrNVrqs/wfHtdIikyF+bcSI0XC+9exC04DP+oCI1EnbVBg==", "8bcd4034-0bd4-4d06-abda-5fbac37f1022" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 21, 32, 330, DateTimeKind.Utc).AddTicks(7275));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 21, 32, 330, DateTimeKind.Utc).AddTicks(7282));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 21, 32, 330, DateTimeKind.Utc).AddTicks(7284));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 21, 32, 330, DateTimeKind.Utc).AddTicks(7288));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 3, 21, 32, 330, DateTimeKind.Utc).AddTicks(7432));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prescription",
                table: "Appointments");

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
    }
}
