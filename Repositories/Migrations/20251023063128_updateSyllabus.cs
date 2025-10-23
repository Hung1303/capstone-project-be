using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateSyllabus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherProfileId",
                table: "Syllabuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Syllabuses_TeacherProfileId",
                table: "Syllabuses",
                column: "TeacherProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabuses_TeacherProfiles_TeacherProfileId",
                table: "Syllabuses",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabuses_TeacherProfiles_TeacherProfileId",
                table: "Syllabuses");

            migrationBuilder.DropIndex(
                name: "IX_Syllabuses_TeacherProfileId",
                table: "Syllabuses");

            migrationBuilder.DropColumn(
                name: "TeacherProfileId",
                table: "Syllabuses");
        }
    }
}
