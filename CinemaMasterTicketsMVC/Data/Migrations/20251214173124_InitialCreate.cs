using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddOns",
                columns: table => new
                {
                    AddOnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOns", x => x.AddOnId);
                });

            migrationBuilder.CreateTable(
                name: "BoxOffices",
                columns: table => new
                {
                    BoxOfficeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoxOfficeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxOffices", x => x.BoxOfficeId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FidelityPoints = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieAPIId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverView = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoxOfficeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_BoxOffices_BoxOfficeId",
                        column: x => x.BoxOfficeId,
                        principalTable: "BoxOffices",
                        principalColumn: "BoxOfficeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    SeatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.SeatId);
                    table.ForeignKey(
                        name: "FK_Seats_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSeats",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSeats", x => new { x.SessionId, x.SeatId });
                    table.ForeignKey(
                        name: "FK_SessionSeats_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "SeatId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionSeats_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    EmailToSend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoldAtBoxOffice = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tickets_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tickets_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAddOns",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AddOnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAddOns", x => new { x.TicketId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_TicketAddOns_AddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "AddOns",
                        principalColumn: "AddOnId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketAddOns_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSeats",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSeats", x => new { x.TicketId, x.SeatId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_TicketSeats_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "SeatId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSeats_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSeats_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AddOns",
                columns: new[] { "AddOnId", "AddonName", "Price" },
                values: new object[,]
                {
                    { 1, "Palomitas Grandes", 5.00m },
                    { 2, "Bebida Grande", 3.50m }
                });

            migrationBuilder.InsertData(
                table: "BoxOffices",
                columns: new[] { "BoxOfficeId", "BoxOfficeName" },
                values: new object[,]
                {
                    { 1, "Taquilla Principal" },
                    { 2, "Taquilla Rápida" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "City", "CreatedDate", "Email", "FidelityPoints", "FirstName", "LastName", "isActive" },
                values: new object[,]
                {
                    { 1, "Calle Falsa 123", "Madrid", new DateTime(2025, 11, 14, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1674), "ana@email.com", 150, "Ana", "Martínez", true },
                    { 2, "Avenida Siempreviva 42", "Barcelona", new DateTime(2025, 11, 29, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1681), "javier@email.com", 50, "Javier", "López", true }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AddedAt", "DurationMinutes", "Genre", "MovieAPIId", "OverView", "PosterUrl", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 7, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1774), 150, "Sci-Fi", "101", "Viaje épico a través de la galaxia.", "/img/cosmic.jpg", "Aventura Cósmica" },
                    { 2, new DateTime(2025, 12, 9, 18, 31, 24, 348, DateTimeKind.Local).AddTicks(1779), 90, "Thriller", "102", "Un detective debe resolver un crimen en una vieja mansión.", "/img/mansion.jpg", "Misterio en la Mansión" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "Capacity" },
                values: new object[,]
                {
                    { 1, 100 },
                    { 2, 50 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "BoxOfficeId", "DNI", "Email", "Firstname", "Lastname" },
                values: new object[,]
                {
                    { 1, 1, "12345678-A", "laura@cinema.com", "Laura", "Gómez" },
                    { 2, 1, "87654321-B", "pedro@cinema.com", "Pedro", "Ruiz" }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "SeatId", "RoomId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 2 },
                    { 12, 2 },
                    { 13, 2 },
                    { 14, 2 },
                    { 15, 2 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "SessionId", "MovieId", "Price", "RoomId", "StartTime", "Status" },
                values: new object[,]
                {
                    { 1, 1, 8.50m, 1, new DateTime(2025, 12, 14, 20, 31, 24, 348, DateTimeKind.Local).AddTicks(1800), "Active" },
                    { 2, 2, 7.00m, 2, new DateTime(2025, 12, 14, 23, 31, 24, 348, DateTimeKind.Local).AddTicks(1807), "Active" }
                });

            migrationBuilder.InsertData(
                table: "SessionSeats",
                columns: new[] { "SeatId", "SessionId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "TicketId", "CustomerId", "EmailToSend", "EmployeeId", "PaymentMethod", "PurchasedAt", "SessionId", "SoldAtBoxOffice", "TotalPrice" },
                values: new object[,]
                {
                    { 1, 1, "ana@email.com", null, "Credit Card", new DateTime(2025, 12, 14, 18, 26, 24, 348, DateTimeKind.Local).AddTicks(1924), 1, false, 13.50m },
                    { 2, null, "guest@email.com", 1, "Cash", new DateTime(2025, 12, 14, 18, 29, 24, 348, DateTimeKind.Local).AddTicks(1932), 1, true, 8.50m }
                });

            migrationBuilder.InsertData(
                table: "TicketAddOns",
                columns: new[] { "AddOnId", "TicketId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "TicketSeats",
                columns: new[] { "SeatId", "SessionId", "TicketId" },
                values: new object[,]
                {
                    { 6, 1, 1 },
                    { 7, 1, 1 },
                    { 8, 1, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BoxOfficeId",
                table: "Employees",
                column: "BoxOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_RoomId",
                table: "Seats",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_MovieId",
                table: "Sessions",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RoomId",
                table: "Sessions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSeats_SeatId",
                table: "SessionSeats",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAddOns_AddOnId",
                table: "TicketAddOns",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerId",
                table: "Tickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EmployeeId",
                table: "Tickets",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SessionId",
                table: "Tickets",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSeats_SeatId",
                table: "TicketSeats",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSeats_SessionId",
                table: "TicketSeats",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionSeats");

            migrationBuilder.DropTable(
                name: "TicketAddOns");

            migrationBuilder.DropTable(
                name: "TicketSeats");

            migrationBuilder.DropTable(
                name: "AddOns");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "BoxOffices");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
