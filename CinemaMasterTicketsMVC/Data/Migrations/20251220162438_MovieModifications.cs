using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovieModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 20, 17, 24, 37, 805, DateTimeKind.Local).AddTicks(1217));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 5, 17, 24, 37, 805, DateTimeKind.Local).AddTicks(1223));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                columns: new[] { "AddedAt", "Director" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 24, 37, 805, DateTimeKind.Local).AddTicks(1323), null });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                columns: new[] { "AddedAt", "Director" },
                values: new object[] { new DateTime(2025, 12, 15, 17, 24, 37, 805, DateTimeKind.Local).AddTicks(1328), null });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2025, 12, 20, 19, 24, 37, 805, DateTimeKind.Local).AddTicks(1348));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "StartTime",
                value: new DateTime(2025, 12, 20, 22, 24, 37, 805, DateTimeKind.Local).AddTicks(1355));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 1,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 20, 17, 19, 37, 805, DateTimeKind.Local).AddTicks(1473));

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 2,
                column: "PurchasedAt",
                value: new DateTime(2025, 12, 20, 17, 22, 37, 805, DateTimeKind.Local).AddTicks(1479));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "Movies");

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
                column: "AddedAt",
                value: new DateTime(2025, 12, 13, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                column: "AddedAt",
                value: new DateTime(2025, 12, 15, 12, 16, 17, 429, DateTimeKind.Local).AddTicks(4085));

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
    }
}
