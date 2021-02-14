using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class AddMedicalIntToPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalInstitutionId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicalInstitutionId",
                table: "Patients",
                column: "MedicalInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalInstitutions_MedicalInstitutionId",
                table: "Patients",
                column: "MedicalInstitutionId",
                principalTable: "MedicalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalInstitutions_MedicalInstitutionId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicalInstitutionId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalInstitutionId",
                table: "Patients");
        }
    }
}
