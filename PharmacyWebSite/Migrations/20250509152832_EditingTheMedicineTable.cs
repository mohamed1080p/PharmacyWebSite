using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebSite.Migrations
{
    /// <inheritdoc />
    public partial class EditingTheMedicineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityInStock",
                table: "Medicines",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "MedicineId",
                table: "Medicines",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Medicines");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Medicines",
                newName: "QuantityInStock");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Medicines",
                newName: "MedicineId");
        }
    }
}
