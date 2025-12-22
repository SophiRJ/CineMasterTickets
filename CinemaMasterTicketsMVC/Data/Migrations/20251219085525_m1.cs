using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 19, 9, 55, 24, 560, DateTimeKind.Local).AddTicks(9479));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 4, 9, 55, 24, 560, DateTimeKind.Local).AddTicks(9485));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                column: "AddedAt",
                value: new DateTime(2025, 12, 12, 9, 55, 24, 560, DateTimeKind.Local).AddTicks(9544));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                column: "AddedAt",
                value: new DateTime(2025, 12, 14, 9, 55, 24, 560, DateTimeKind.Local).AddTicks(9547));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2025, 12, 19, 11, 55, 24, 560, DateTimeKind.Local).AddTicks(9605));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "StartTime",
                value: new DateTime(2025, 12, 19, 14, 55, 24, 560, DateTimeKind.Local).AddTicks(9610));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 1,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 19, 9, 50, 24, 560, DateTimeKind.Local).AddTicks(9666));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 2,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 19, 9, 53, 24, 560, DateTimeKind.Local).AddTicks(9670));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 14, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1674));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 29, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1681));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                column: "AddedAt",
                value: new DateTime(2025, 12, 7, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1774));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                column: "AddedAt",
                value: new DateTime(2025, 12, 9, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1779));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2025, 12, 14, 20, 31, 24, 348, DateTimeKind.Local).AddTicks(1800));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "StartTime",
                value: new DateTime(2025, 12, 14, 23, 31, 24, 348, DateTimeKind.Local).AddTicks(1807));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 1,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 14, 18, 26, 24, 348, DateTimeKind.Local).AddTicks(1924));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 2,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 14, 18, 29, 24, 348, DateTimeKind.Local).AddTicks(1932));
        }
    }
}
