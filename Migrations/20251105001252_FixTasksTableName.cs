using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanAppApi.Migrations
{
    /// <inheritdoc />
    public partial class FixTasksTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "tasks");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ColumnId",
                table: "tasks",
                newName: "IX_tasks_ColumnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tasks",
                table: "tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subTasks_tasks_BoardTaskId",
                table: "subTasks",
                column: "BoardTaskId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_columns_ColumnId",
                table: "tasks",
                column: "ColumnId",
                principalTable: "columns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subTasks_tasks_BoardTaskId",
                table: "subTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_columns_ColumnId",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tasks",
                table: "tasks");

            migrationBuilder.RenameTable(
                name: "tasks",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_ColumnId",
                table: "Tasks",
                newName: "IX_Tasks_ColumnId");

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
    }
}
