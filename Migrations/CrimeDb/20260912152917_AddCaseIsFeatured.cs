using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UKCrimeWeb.Migrations.CrimeDb
{
    /// <inheritdoc />
    public partial class AddCaseIsFeatured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Case",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Case");
        }
    }
}