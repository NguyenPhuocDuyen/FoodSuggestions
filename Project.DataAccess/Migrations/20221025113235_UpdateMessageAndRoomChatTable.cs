using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DataAccess.Migrations
{
    public partial class UpdateMessageAndRoomChatTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_RoomChat_RoomId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_FromUserId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomChat_Users_UserId",
                table: "RoomChat");

            migrationBuilder.DropColumn(
                name: "Date_Create",
                table: "RoomChat");

            migrationBuilder.DropColumn(
                name: "Date_Edit",
                table: "RoomChat");

            migrationBuilder.DropColumn(
                name: "ReplyStatus",
                table: "RoomChat");

            migrationBuilder.DropColumn(
                name: "Date_Edit",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RoomChat",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomChat_UserId",
                table: "RoomChat",
                newName: "IX_RoomChat_AdminId");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Message",
                newName: "ToRoomId");

            migrationBuilder.RenameColumn(
                name: "Date_Create",
                table: "Message",
                newName: "Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Message_RoomId",
                table: "Message",
                newName: "IX_Message_ToRoomId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoomChat",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "Message",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_RoomChat_ToRoomId",
                table: "Message",
                column: "ToRoomId",
                principalTable: "RoomChat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_FromUserId",
                table: "Message",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomChat_Users_AdminId",
                table: "RoomChat",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_RoomChat_ToRoomId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_FromUserId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomChat_Users_AdminId",
                table: "RoomChat");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "RoomChat",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomChat_AdminId",
                table: "RoomChat",
                newName: "IX_RoomChat_UserId");

            migrationBuilder.RenameColumn(
                name: "ToRoomId",
                table: "Message",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Message",
                newName: "Date_Create");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ToRoomId",
                table: "Message",
                newName: "IX_Message_RoomId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoomChat",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Create",
                table: "RoomChat",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Edit",
                table: "RoomChat",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplyStatus",
                table: "RoomChat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Edit",
                table: "Message",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_RoomChat_RoomId",
                table: "Message",
                column: "RoomId",
                principalTable: "RoomChat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_FromUserId",
                table: "Message",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomChat_Users_UserId",
                table: "RoomChat",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
