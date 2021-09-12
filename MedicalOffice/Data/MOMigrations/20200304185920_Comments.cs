using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalOffice.Data.MOMigrations
{
    public partial class Comments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "MO",
                table: "PatientConditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "MO",
                table: "PatientConditions");
        }
    }
}
