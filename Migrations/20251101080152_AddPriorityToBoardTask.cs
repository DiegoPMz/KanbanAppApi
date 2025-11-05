using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanAppApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityToBoardTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boardTasks_columns_ColumnId",
                table: "boardTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_subTasks_boardTasks_BoardTaskId",
                table: "subTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_boardTasks",
                table: "boardTasks");

            migrationBuilder.RenameTable(
                name: "boardTasks",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_boardTasks_ColumnId",
                table: "Tasks",
                newName: "IX_Tasks_ColumnId");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subTasks_Tasks_BoardTaskId",
                table: "subTasks",
                column: "BoardTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_columns_ColumnId",
                table: "Tasks",
                column: "ColumnId",
                principalTable: "columns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subTasks_Tasks_BoardTaskId",
                table: "subTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_columns_ColumnId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "boardTasks");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ColumnId",
                table: "boardTasks",
                newName: "IX_boardTasks_ColumnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_boardTasks",
                table: "boardTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_boardTasks_columns_ColumnId",
                table: "boardTasks",
                column: "ColumnId",
                principalTable: "columns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subTasks_boardTasks_BoardTaskId",
                table: "subTasks",
                column: "BoardTaskId",
                principalTable: "boardTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
