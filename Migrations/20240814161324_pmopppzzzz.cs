using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_plateforme_de_recurtement.Migrations
{
    /// <inheritdoc />
    public partial class pmopppzzzz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewDate",
                table: "rescheduleRequesttests");

            migrationBuilder.DropColumn(
                name: "NewTime",
                table: "rescheduleRequesttests");

            migrationBuilder.AddColumn<DateTime>(
                name: "NewDateTime",
                table: "rescheduleRequesttests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewDateTime",
                table: "rescheduleRequesttests");

            migrationBuilder.AddColumn<string>(
                name: "NewDate",
                table: "rescheduleRequesttests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NewTime",
                table: "rescheduleRequesttests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
