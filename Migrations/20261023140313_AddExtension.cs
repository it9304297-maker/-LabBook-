using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPageBase.Migrations
{
    /// <inheritdoc />
    public partial class AddExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Instrument",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 1,
                column: "Extension",
                value: "2301");

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 2,
                column: "Extension",
                value: "2302");

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 3,
                column: "Extension",
                value: "5101");

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 4,
                column: "Extension",
                value: "2301");

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 5,
                column: "Extension",
                value: "3304");

            migrationBuilder.UpdateData(
                table: "Instrument",
                keyColumn: "Id",
                keyValue: 6,
                column: "Extension",
                value: "2302");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Instrument");
        }
    }
}
