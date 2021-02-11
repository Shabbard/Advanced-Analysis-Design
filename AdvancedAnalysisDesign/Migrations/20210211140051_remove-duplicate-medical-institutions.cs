using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class removeduplicatemedicalinstitutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralPractitioners_MedicalInstitution_SurgeryId",
                table: "GeneralPractitioners");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_MedicalInstitution_PharmacyId",
                table: "Pharmacists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalInstitution",
                table: "MedicalInstitution");

            migrationBuilder.RenameTable(
                name: "MedicalInstitution",
                newName: "MedicalInstitutions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalInstitutions",
                table: "MedicalInstitutions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralPractitioners_MedicalInstitutions_SurgeryId",
                table: "GeneralPractitioners",
                column: "SurgeryId",
                principalTable: "MedicalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_MedicalInstitutions_PharmacyId",
                table: "Pharmacists",
                column: "PharmacyId",
                principalTable: "MedicalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralPractitioners_MedicalInstitutions_SurgeryId",
                table: "GeneralPractitioners");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_MedicalInstitutions_PharmacyId",
                table: "Pharmacists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalInstitutions",
                table: "MedicalInstitutions");

            migrationBuilder.RenameTable(
                name: "MedicalInstitutions",
                newName: "MedicalInstitution");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalInstitution",
                table: "MedicalInstitution",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralPractitioners_MedicalInstitution_SurgeryId",
                table: "GeneralPractitioners",
                column: "SurgeryId",
                principalTable: "MedicalInstitution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_MedicalInstitution_PharmacyId",
                table: "Pharmacists",
                column: "PharmacyId",
                principalTable: "MedicalInstitution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
