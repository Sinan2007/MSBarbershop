using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSBarbershop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Barbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Barbers");
        }
    }
}
