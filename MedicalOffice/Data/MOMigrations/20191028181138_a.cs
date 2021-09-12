using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "MO",
                table: "Patients",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "MO",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "MO",
                table: "Patients",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                schema: "MO",
                table: "Patients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                schema: "MO",
                table: "Patients");
        }
    }
}
