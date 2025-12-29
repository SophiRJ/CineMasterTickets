using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIsActiveToAddOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AddOns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AddOns");
        }
    }
}
