using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ITravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "515d9411-fa4f-4934-aa8b-33378faedb29", null, "Administrator", "Administrator" },
                    { "d8aae7d3-5a35-4ef2-82fc-c0aa1c46876f", null, "Provider", "Provider" },
                    { "e7d01558-bc8e-447a-925f-b43242c9e927", null, "Users", "Users" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "515d9411-fa4f-4934-aa8b-33378faedb29");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8aae7d3-5a35-4ef2-82fc-c0aa1c46876f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7d01558-bc8e-447a-925f-b43242c9e927");
        }
    }
}
