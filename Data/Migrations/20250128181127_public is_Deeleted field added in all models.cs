using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class publicis_Deeletedfieldaddedinallmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bloggers_Categories_CategoryId",
                table: "Bloggers");

            migrationBuilder.DropIndex(
                name: "IX_Bloggers_CategoryId",
                table: "Bloggers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Bloggers");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Deleted",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Deleted",
                table: "Comments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Deleted",
                table: "Categories",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Deleted",
                table: "Bloggers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Deleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Is_Deleted",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Is_Deleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Is_Deleted",
                table: "Bloggers");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Bloggers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bloggers_CategoryId",
                table: "Bloggers",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bloggers_Categories_CategoryId",
                table: "Bloggers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }
    }
}
