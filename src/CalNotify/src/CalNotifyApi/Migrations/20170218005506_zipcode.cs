using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace CalNotifyApi.Migrations
{
    public partial class zipcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ZipCodes");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ZipCodes");

            migrationBuilder.AddColumn<PostgisPoint>(
                name: "Location",
                table: "ZipCodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "ZipCodes");

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "ZipCodes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "ZipCodes",
                nullable: true);
        }
    }
}
