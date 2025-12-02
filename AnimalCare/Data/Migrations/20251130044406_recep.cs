using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class recep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceptionistId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Receptionists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptionists", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "ReceptionistId", "SecurityStamp" },
                values: new object[] { "c6170938-3b8a-4d60-b83e-67f116e4585a", new DateTime(2025, 11, 30, 4, 44, 5, 132, DateTimeKind.Utc).AddTicks(3038), "AQAAAAIAAYagAAAAEEORDnVd9wjd02kSxQ7ypvGtho11Waebz0OF4beyZrtvxu3INWcAjJboIFd7YRrGdw==", null, "72e807ca-9ebc-49c8-88d0-f5d25ddc73b7" });

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ReceptionistId",
                table: "AspNetUsers",
                column: "ReceptionistId",
                unique: true,
                filter: "[ReceptionistId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Receptionists_ReceptionistId",
                table: "AspNetUsers",
                column: "ReceptionistId",
                principalTable: "Receptionists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Receptionists_ReceptionistId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Receptionists");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ReceptionistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ReceptionistId",
                table: "AspNetUsers");

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
    }
}
