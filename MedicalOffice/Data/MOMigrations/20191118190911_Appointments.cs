using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class Appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApptReasons",
                schema: "MO",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReasonName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApptReasons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                schema: "MO",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notes = table.Column<string>(maxLength: 2000, nullable: false),
                    appDate = table.Column<DateTime>(nullable: false),
                    PatientID = table.Column<int>(nullable: false),
                    extraFee = table.Column<decimal>(nullable: false),
                    ApptReasonID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Appointments_ApptReasons_ApptReasonID",
                        column: x => x.ApptReasonID,
                        principalSchema: "MO",
                        principalTable: "ApptReasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientID",
                        column: x => x.PatientID,
                        principalSchema: "MO",
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ApptReasonID",
                schema: "MO",
                table: "Appointments",
                column: "ApptReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientID",
                schema: "MO",
                table: "Appointments",
                column: "PatientID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments",
                schema: "MO");

            migrationBuilder.DropTable(
                name: "ApptReasons",
                schema: "MO");
        }
    }
}
