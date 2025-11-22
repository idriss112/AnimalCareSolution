using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "0a81961f-1e70-46a5-8d0e-168ad7ca44b1", new DateTime(2025, 11, 21, 23, 9, 23, 139, DateTimeKind.Utc).AddTicks(8453), "admin@animalcare.com", "ADMIN@ANIMALCARE.COM", "ADMIN@ANIMALCARE.COM", "AQAAAAIAAYagAAAAEK+xjTRfw7Ide9hI4dYBhiWa2AgdZ+MCIBSoW1VAM8i2lGWCRK9enBwKDLs8w5ZKqQ==", "f37f661d-813e-4bf5-9fe2-280c98e9dc48", "admin@animalcare.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "e6d43122-1d62-4a82-92f6-c24e7dcd45c3", new DateTime(2025, 11, 21, 23, 4, 39, 511, DateTimeKind.Utc).AddTicks(7589), "admin@animalcare.local", "ADMIN@ANIMALCARE.LOCAL", "ADMIN@ANIMALCARE.LOCAL", "AQAAAAIAAYagAAAAEBA84BYL6+aG20IlTKqYOgnYRGUVfFynLzp6jpy6txBeJGTUJJVVSLEc5faLmpufKQ==", "f2f36bb6-d759-44ab-a3a8-599dfff401fe", "admin@animalcare.local" });
        }
    }
}
