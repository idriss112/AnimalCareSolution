using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cdcf0c5-d156-4fb5-a872-eeead8a29862", new DateTime(2025, 11, 29, 5, 7, 17, 101, DateTimeKind.Utc).AddTicks(5833), "AQAAAAIAAYagAAAAEBGTQMAqfroGbnkmCyg62aSDLynN4xLFVKGAvAoRg7VqIbZdkN6m7OYr8+zIzgY0IQ==", "3dca77fe-3a20-4e30-abfb-728e0cc1b557" });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "215 Rue Sainte-Catherine Ouest, Montréal, QC", new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3591), "sarah.tremblay@example.com", "Sarah", "Tremblay", "514-987-2234" },
                    { 2, "88 Av. du Mont-Royal Est, Montréal, QC", new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3596), "julien.moreau@example.com", "Julien", "Moreau", "438-771-9023" },
                    { 3, "4020 Boulevard Décarie, Montréal, QC", new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3601), "amira.haddad@example.com", "Amira", "El-Haddad", "514-622-3381" },
                    { 4, "1200 Rue Sherbrooke Ouest, Montréal, QC", new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3603), "kevin.ouellet@example.com", "Kevin", "Ouellet", "581-300-7709" },
                    { 5, "59 Rue Jean-Talon Est, Montréal, QC", new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3606), "layla.benali@example.com", "Layla", "Benali", "438-245-1940" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8d40216-b331-4cf8-a9b1-05668e374421", new DateTime(2025, 11, 28, 21, 36, 1, 860, DateTimeKind.Utc).AddTicks(4431), "AQAAAAIAAYagAAAAEB353YagQRZzG/QfLA56yUpG/s5iVy//AtObdEveqFkiZkkjhJiJgujVrVVh2fcHUQ==", "a95e9a75-b7de-4941-8a5b-3cd7b4f9b015" });
        }
    }
}
