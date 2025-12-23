using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BoxOffices",
                columns: new[] { "BoxOfficeId", "BoxOfficeName" },
                values: new object[,]
                {
                    { 1, "Taquilla Principal - Entrada" },
                    { 2, "Taquilla Lateral - Parking" },
                    { 3, "Taquilla VIP - Planta 1" },
                    { 4, "Taquilla Online - Recogida" },
                    { 5, "Taquilla Express - Kiosko" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BoxOffices",
                keyColumn: "BoxOfficeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BoxOffices",
                keyColumn: "BoxOfficeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BoxOffices",
                keyColumn: "BoxOfficeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BoxOffices",
                keyColumn: "BoxOfficeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BoxOffices",
                keyColumn: "BoxOfficeId",
                keyValue: 5);
        }
    }
}
