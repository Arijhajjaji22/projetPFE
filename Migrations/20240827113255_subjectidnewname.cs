﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_plateforme_de_recurtement.Migrations
{
    /// <inheritdoc />
    public partial class subjectidnewname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfferId",
                table: "rescheduleRequesttests",
                newName: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "rescheduleRequesttests",
                newName: "OfferId");
        }
    }
}
