using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_plateforme_de_recurtement.Migrations
{
    /// <inheritdoc />
    public partial class subjectidnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "rescheduleRequesttests");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "rescheduleRequesttests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "rescheduleRequesttests");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "rescheduleRequesttests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
