using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class Picture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "imageContent",
                schema: "MO",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageFileName",
                schema: "MO",
                table: "Patients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageMimeType",
                schema: "MO",
                table: "Patients",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageContent",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "imageFileName",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "imageMimeType",
                schema: "MO",
                table: "Patients");
        }
    }
}
