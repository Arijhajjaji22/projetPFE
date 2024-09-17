using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_plateforme_de_recurtement.Migrations
{
    /// <inheritdoc />
    public partial class entretienpassé : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InterviewPassed",
                table: "Forms",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewPassed",
                table: "Forms");
        }
    }
}
