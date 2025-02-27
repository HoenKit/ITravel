using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Bookings_BookingId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Providers_ProviderId",
                table: "Tours");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Bookings_BookingId",
                table: "Customers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Providers_ProviderId",
                table: "Tours",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Bookings_BookingId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Providers_ProviderId",
                table: "Tours");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Bookings_BookingId",
                table: "Customers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Providers_ProviderId",
                table: "Tours",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
