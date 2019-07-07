using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoItApi.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    TaskDescription = table.Column<string>(nullable: false),
                    DueDateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertTime",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Time = table.Column<DateTimeOffset>(nullable: false),
                    TaskId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertTime_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    InsertedTime = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(nullable: false),
                    TaskId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertTime_TaskId",
                table: "AlertTime",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TaskId",
                table: "Comment",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertTime");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
