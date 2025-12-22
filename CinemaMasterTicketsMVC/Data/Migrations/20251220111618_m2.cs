using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackdropUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 20, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(3869));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 5, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(3875));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                columns: new[] { "AddedAt", "BackdropUrl" },
                values: new object[] { new DateTime(2025, 12, 13, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(4079), null });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                columns: new[] { "AddedAt", "BackdropUrl" },
                values: new object[] { new DateTime(2025, 12, 15, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(4085), null });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2025, 12, 20, 14, 16, 17, 429, DateTimeKind.Local).AddTicks(4105));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "StartTime",
                value: new DateTime(2025, 12, 20, 17, 16, 17, 429, DateTimeKind.Local).AddTicks(4112));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 1,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 20, 12, 11, 17, 429, DateTimeKind.Local).AddTicks(4193));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 2,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 20, 12, 14, 17, 429, DateTimeKind.Local).AddTicks(4199));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropUrl",
                table: "Movies");

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
    }
}
