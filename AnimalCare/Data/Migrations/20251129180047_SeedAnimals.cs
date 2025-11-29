using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnimals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Breed", "CreatedAt", "DateOfBirth", "ImportantNotes", "Name", "OwnerId", "Sex", "Species", "Weight" },
                values: new object[,]
                {
                    { 1, "Labrador Retriever", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2019, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Very friendly, good with children. Up to date on vaccines.", "Bella", 1, "Female", "Dog", 28.5m },
                    { 2, "German Shepherd", new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2018, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Needs regular exercise. Slight anxiety with strangers.", "Max", 1, "Male", "Dog", 32.2m },
                    { 3, "Siamese", new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Indoor-only cat. Sensitive stomach, special food required.", "Luna", 2, "Female", "Cat", 4.3m },
                    { 4, "British Shorthair", new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2021, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Calm temperament. Slight overweight, on diet plan.", "Milo", 3, "Male", "Cat", 5.1m },
                    { 5, "Bulldog", new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2017, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brachycephalic, monitor breathing during exercise.", "Rocky", 3, "Male", "Dog", 24.8m },
                    { 6, "Cockatiel", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Very vocal in the morning. Needs regular wing checks.", "Coco", 4, "Female", "Bird", 0.09m },
                    { 7, "Golden Retriever", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2020, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allergic to chicken-based food. Use hypoallergenic treats.", "Nala", 5, "Female", "Dog", 26.7m }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cdcf0c5-d156-4fb5-a872-eeead8a29862", new DateTime(2025, 11, 29, 5, 7, 17, 101, DateTimeKind.Utc).AddTicks(5833), "AQAAAAIAAYagAAAAEBGTQMAqfroGbnkmCyg62aSDLynN4xLFVKGAvAoRg7VqIbZdkN6m7OYr8+zIzgY0IQ==", "3dca77fe-3a20-4e30-abfb-728e0cc1b557" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3591));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3596));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3601));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3603));

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 5, 7, 17, 161, DateTimeKind.Utc).AddTicks(3606));
        }
    }
}
