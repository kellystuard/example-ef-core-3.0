using Microsoft.EntityFrameworkCore.Migrations;

namespace Examples.EFCore.Complete.Migrations
{
    public partial class Addingvisibleproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Users");
        }
    }
}
