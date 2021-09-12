using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class MedTrials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalTrialID",
                schema: "MO",
                table: "Patients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalTrials",
                schema: "MO",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrialName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTrials", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicalTrialID",
                schema: "MO",
                table: "Patients",
                column: "MedicalTrialID");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalTrials_MedicalTrialID",
                schema: "MO",
                table: "Patients",
                column: "MedicalTrialID",
                principalSchema: "MO",
                principalTable: "MedicalTrials",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalTrials_MedicalTrialID",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "MedicalTrials",
                schema: "MO");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicalTrialID",
                schema: "MO",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalTrialID",
                schema: "MO",
                table: "Patients");
        }
    }
}
