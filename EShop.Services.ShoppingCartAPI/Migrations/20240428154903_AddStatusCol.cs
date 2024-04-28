using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CartHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CartHeaders");
        }
    }
}
