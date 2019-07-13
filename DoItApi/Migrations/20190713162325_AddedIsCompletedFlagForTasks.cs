using Microsoft.EntityFrameworkCore.Migrations;

namespace DoItApi.Migrations
{
    public partial class AddedIsCompletedFlagForTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Tasks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Tasks");
        }
    }
}
