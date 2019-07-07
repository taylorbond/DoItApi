using Microsoft.EntityFrameworkCore.Migrations;

namespace DoItApi.Migrations
{
    public partial class TaskNameChangedToDiaTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertTime_Tasks_TaskId",
                table: "AlertTime");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tasks_TaskId",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Comment",
                newName: "DiaTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TaskId",
                table: "Comment",
                newName: "IX_Comment_DiaTaskId");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "AlertTime",
                newName: "DiaTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertTime_TaskId",
                table: "AlertTime",
                newName: "IX_AlertTime_DiaTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertTime_Tasks_DiaTaskId",
                table: "AlertTime",
                column: "DiaTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tasks_DiaTaskId",
                table: "Comment",
                column: "DiaTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertTime_Tasks_DiaTaskId",
                table: "AlertTime");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tasks_DiaTaskId",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "DiaTaskId",
                table: "Comment",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_DiaTaskId",
                table: "Comment",
                newName: "IX_Comment_TaskId");

            migrationBuilder.RenameColumn(
                name: "DiaTaskId",
                table: "AlertTime",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_AlertTime_DiaTaskId",
                table: "AlertTime",
                newName: "IX_AlertTime_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertTime_Tasks_TaskId",
                table: "AlertTime",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tasks_TaskId",
                table: "Comment",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
