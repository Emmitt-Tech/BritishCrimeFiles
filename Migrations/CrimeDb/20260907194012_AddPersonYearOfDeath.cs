using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UKCrimeWeb.Migrations.CrimeDb
{
    /// <inheritdoc />
    public partial class AddPersonYearOfDeath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfDeath",
                table: "Person",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfDeath",
                table: "Person");
        }
    }
}
