using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class correctNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNUmber",
                table: "Companies",
                newName: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Companies",
                newName: "PhoneNUmber");
        }
    }
}
