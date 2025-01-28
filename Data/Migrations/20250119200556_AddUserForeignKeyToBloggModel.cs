using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserForeignKeyToBloggModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bloggers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Bloggers_UserId",
                table: "Bloggers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bloggers_Users_UserId",
                table: "Bloggers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bloggers_Users_UserId",
                table: "Bloggers");

            migrationBuilder.DropIndex(
                name: "IX_Bloggers_UserId",
                table: "Bloggers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Bloggers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
