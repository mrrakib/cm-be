using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COLLECTION_MANAGEMENT_REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class AlterTblUserAddCompanyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "users",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "users");
        }
    }
}
