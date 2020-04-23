﻿#pragma warning disable 1591
using Microsoft.EntityFrameworkCore.Migrations;

namespace Examples.EFCore.Complete.Migrations
{
	public partial class Initial_Migration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					FirstName = table.Column<string>(nullable: false),
					LastName = table.Column<string>(nullable: false),
					Visible = table.Column<bool>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}

#pragma warning restore 1591
