using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassDescription",
                table: "ClassSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "ClassSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-aaaa-bbbb-111111111111"),
                columns: new[] { "ClassDescription", "ClassName" },
                values: new object[] { "A", "ClassA" });

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-aaaa-bbbb-222222222222"),
                columns: new[] { "ClassDescription", "ClassName" },
                values: new object[] { "A", "ClassA" });

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-3333-aaaa-bbbb-333333333333"),
                columns: new[] { "ClassDescription", "ClassName" },
                values: new object[] { "B", "ClassB" });

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-4444-aaaa-bbbb-444444444444"),
                columns: new[] { "ClassDescription", "ClassName" },
                values: new object[] { "B", "ClassB" });

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-5555-aaaa-bbbb-555555555555"),
                columns: new[] { "ClassDescription", "ClassName" },
                values: new object[] { "C", "ClassC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassDescription",
                table: "ClassSchedules");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "ClassSchedules");
        }
    }
}
