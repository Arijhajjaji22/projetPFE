using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_plateforme_de_recurtement.Migrations
{
    /// <inheritdoc />
    public partial class adduserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "availabilityConfirmations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "availabilityConfirmations");
        }
    }
}
