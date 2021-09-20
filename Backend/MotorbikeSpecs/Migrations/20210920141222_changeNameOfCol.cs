using Microsoft.EntityFrameworkCore.Migrations;

namespace MotorbikeSpecs.Migrations
{
    public partial class changeNameOfCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "BraapUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                newName: "IX_Reviews_BraapUserId");

            migrationBuilder.CreateTable(
                name: "BraapUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GitHub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURI = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BraapUsers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_BraapUsers_BraapUserId",
                table: "Reviews",
                column: "BraapUserId",
                principalTable: "BraapUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_BraapUsers_BraapUserId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "BraapUsers");

            migrationBuilder.RenameColumn(
                name: "BraapUserId",
                table: "Reviews",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_BraapUserId",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GitHub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
