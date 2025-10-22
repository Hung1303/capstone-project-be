using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Courses_CourseId",
                table: "ClassSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Subjects_SubjectId",
                table: "ClassSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Courses_CourseId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_TeacherProfiles_TeacherProfileId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_CourseId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_TeacherProfileId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSchedules_CourseId",
                table: "ClassSchedules");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "TeacherProfileId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ClassSchedules");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "ClassSchedules",
                newName: "TeacherProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassSchedules_SubjectId",
                table: "ClassSchedules",
                newName: "IX_ClassSchedules_TeacherProfileId");

            migrationBuilder.CreateTable(
                name: "SubjectBuilder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectBuilder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_ClassSchedules_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_ClassScheduleId",
                table: "SubjectBuilder",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_CourseId",
                table: "SubjectBuilder",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_SubjectId",
                table: "SubjectBuilder",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_TeacherProfiles_TeacherProfileId",
                table: "ClassSchedules",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_TeacherProfiles_TeacherProfileId",
                table: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "SubjectBuilder");

            migrationBuilder.RenameColumn(
                name: "TeacherProfileId",
                table: "ClassSchedules",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassSchedules_TeacherProfileId",
                table: "ClassSchedules",
                newName: "IX_ClassSchedules_SubjectId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Subjects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherProfileId",
                table: "Subjects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "ClassSchedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CourseId",
                table: "Subjects",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TeacherProfileId",
                table: "Subjects",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_CourseId",
                table: "ClassSchedules",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Courses_CourseId",
                table: "ClassSchedules",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Subjects_SubjectId",
                table: "ClassSchedules",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Courses_CourseId",
                table: "Subjects",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_TeacherProfiles_TeacherProfileId",
                table: "Subjects",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
