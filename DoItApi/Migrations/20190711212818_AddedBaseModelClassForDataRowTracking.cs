using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoItApi.Migrations
{
    public partial class AddedBaseModelClassForDataRowTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "InsertedTime",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Comment",
                newName: "UpdatedDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AlertTime",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AlertTime",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AlertTime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AlertTime");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AlertTime");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AlertTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Comment",
                newName: "UpdatedTime");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "InsertedTime",
                table: "Comment",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
