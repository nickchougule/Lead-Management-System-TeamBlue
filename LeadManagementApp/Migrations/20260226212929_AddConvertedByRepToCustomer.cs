using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddConvertedByRepToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConvertedByRepId",
                table: "Customers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedByRepId",
                table: "Customers");
        }
    }
}
