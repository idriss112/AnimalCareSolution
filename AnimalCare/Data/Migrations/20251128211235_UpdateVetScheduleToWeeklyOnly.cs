using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVetScheduleToWeeklyOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "VetSchedules");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "VetSchedules");

            migrationBuilder.AlterColumn<int>(
                name: "DayOfWeek",
                table: "VetSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f9163576-d3e1-4b96-9b6d-8681ce7a9036", new DateTime(2025, 11, 28, 21, 12, 33, 941, DateTimeKind.Utc).AddTicks(4668), "AQAAAAIAAYagAAAAEKr9ubLTtuyFUMzmDLvhI1qpyesJpSsbkR5bh1CITJ7/YnLEcM+xCRlqvZlrPqOCJw==", "b0a23477-c8cf-45c2-a08d-ba2b3d62e5e5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DayOfWeek",
                table: "VetSchedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "VetSchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "VetSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a81961f-1e70-46a5-8d0e-168ad7ca44b1", new DateTime(2025, 11, 21, 23, 9, 23, 139, DateTimeKind.Utc).AddTicks(8453), "AQAAAAIAAYagAAAAEK+xjTRfw7Ide9hI4dYBhiWa2AgdZ+MCIBSoW1VAM8i2lGWCRK9enBwKDLs8w5ZKqQ==", "f37f661d-813e-4bf5-9fe2-280c98e9dc48" });
        }
    }
}
