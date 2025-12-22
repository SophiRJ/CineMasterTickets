using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaMasterTicketsMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImageAddOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddOnImage",
                table: "AddOns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddOnImage",
                table: "AddOns");
        }
    }
}
