using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COLLECTION_MANAGEMENT_REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UserPropAlter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "ContactNo",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNo",
                table: "users");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
