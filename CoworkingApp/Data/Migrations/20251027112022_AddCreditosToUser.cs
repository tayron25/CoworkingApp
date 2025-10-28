using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoworkingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditosToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditosDisponibles",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditosDisponibles",
                table: "AspNetUsers");
        }
    }
}
