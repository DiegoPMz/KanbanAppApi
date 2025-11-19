using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanAppApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnUserSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Sub_Id",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_Sub",
                table: "users",
                column: "Sub",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Sub",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_Sub_Id",
                table: "users",
                columns: new[] { "Sub", "Id" },
                unique: true);
        }
    }
}
