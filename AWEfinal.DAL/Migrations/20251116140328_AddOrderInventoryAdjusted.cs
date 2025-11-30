using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWEfinal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderInventoryAdjusted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InventoryAdjusted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryAdjusted",
                table: "Orders");
        }
    }
}
