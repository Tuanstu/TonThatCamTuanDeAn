using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TonThatCamTuanDeAn.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Order",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Order",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Order",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Order");
        }
    }
}
