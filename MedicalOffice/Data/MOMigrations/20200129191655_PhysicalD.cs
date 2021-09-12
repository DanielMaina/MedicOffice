using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class PhysicalD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorPhysicalDescriptions",
                schema: "MO",
                columns: table => new
                {
                    DoctorID = table.Column<int>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    HairColour = table.Column<string>(maxLength: 50, nullable: true),
                    IdentifyingMarks = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorPhysicalDescriptions", x => x.DoctorID);
                    table.ForeignKey(
                        name: "FK_DoctorPhysicalDescriptions_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalSchema: "MO",
                        principalTable: "Doctors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorPhysicalDescriptions",
                schema: "MO");
        }
    }
}
