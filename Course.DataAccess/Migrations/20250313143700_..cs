using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayamentStatus",
                table: "OrderHeaders",
                newName: "PaymentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "OrderHeaders",
                newName: "PayamentStatus");
        }
    }
}
