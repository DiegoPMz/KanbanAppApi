using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanAppApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskIdsDefaultValueToSqlEmptyArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid[]>(
                name: "task_ids",
                table: "columns",
                type: "uuid[]",
                nullable: false,
                defaultValueSql: "'{}'",
                oldClrType: typeof(Guid[]),
                oldType: "uuid[]",
                oldDefaultValue: new Guid[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid[]>(
                name: "task_ids",
                table: "columns",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0],
                oldClrType: typeof(Guid[]),
                oldType: "uuid[]",
                oldDefaultValueSql: "'{}'");
        }
    }
}
