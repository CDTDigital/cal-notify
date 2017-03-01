using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace CalNotifyApi.Migrations
{
    public partial class removezip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZipCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZipCodes",
                columns: table => new
                {
                    Zipcode = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Location = table.Column<PostgisPoint>(nullable: true),
                    Region = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCodes", x => x.Zipcode);
                });
        }
    }
}
