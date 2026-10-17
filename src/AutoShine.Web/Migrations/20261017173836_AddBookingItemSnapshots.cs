using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoShine.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingItemSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceNameSnapshot",
                table: "BookingItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeSnapshot",
                table: "BookingItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "BookingItems",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceNameSnapshot",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "ServiceTypeSnapshot",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "BookingItems");
        }
    }
}
