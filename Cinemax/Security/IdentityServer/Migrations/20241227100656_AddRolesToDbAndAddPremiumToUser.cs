using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToDbAndAddPremiumToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ffd46fd-64d9-49d6-8864-01e494ba7c2d", null, "Admin", "ADMIN" },
                    { "d5a7d5e3-e848-4c55-b3f0-615d6620b6ef", null, "Buyer", "BUYER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ffd46fd-64d9-49d6-8864-01e494ba7c2d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d5a7d5e3-e848-4c55-b3f0-615d6620b6ef");

            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "AspNetUsers");
        }
    }
}
