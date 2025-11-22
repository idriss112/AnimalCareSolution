using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Veterinarians_LinkedVeterinarianId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LinkedVeterinarianId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LinkedVeterinarianId",
                table: "AspNetUsers",
                newName: "VeterinarianId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "IsActive", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6d43122-1d62-4a82-92f6-c24e7dcd45c3", new DateTime(2025, 11, 21, 23, 4, 39, 511, DateTimeKind.Utc).AddTicks(7589), true, "AQAAAAIAAYagAAAAEBA84BYL6+aG20IlTKqYOgnYRGUVfFynLzp6jpy6txBeJGTUJJVVSLEc5faLmpufKQ==", "f2f36bb6-d759-44ab-a3a8-599dfff401fe" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VeterinarianId",
                table: "AspNetUsers",
                column: "VeterinarianId",
                unique: true,
                filter: "[VeterinarianId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Veterinarians_VeterinarianId",
                table: "AspNetUsers",
                column: "VeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Veterinarians_VeterinarianId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VeterinarianId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "VeterinarianId",
                table: "AspNetUsers",
                newName: "LinkedVeterinarianId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0b19ff70-b33f-49c1-9dd6-ac61dee01c55", "AQAAAAIAAYagAAAAEPc5eaYyK+lLY5Z/Fs22HGqqw2h7+/DkOXA+6w6M+e3fOH0kJ+q0cTmeRFrCDhMvtw==", "dc57d227-e264-48ea-a76c-597666c798f5" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LinkedVeterinarianId",
                table: "AspNetUsers",
                column: "LinkedVeterinarianId",
                unique: true,
                filter: "[LinkedVeterinarianId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Veterinarians_LinkedVeterinarianId",
                table: "AspNetUsers",
                column: "LinkedVeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
