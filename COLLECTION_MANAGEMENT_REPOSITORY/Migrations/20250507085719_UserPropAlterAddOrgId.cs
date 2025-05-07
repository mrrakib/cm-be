using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COLLECTION_MANAGEMENT_REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UserPropAlterAddOrgId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrganizationId",
                table: "users",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "users");
        }
    }
}
