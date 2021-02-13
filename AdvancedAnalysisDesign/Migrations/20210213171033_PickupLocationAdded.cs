using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class PickupLocationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalInstitutionId",
                table: "Pickups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_MedicalInstitutionId",
                table: "Pickups",
                column: "MedicalInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pickups_MedicalInstitutions_MedicalInstitutionId",
                table: "Pickups",
                column: "MedicalInstitutionId",
                principalTable: "MedicalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pickups_MedicalInstitutions_MedicalInstitutionId",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_Pickups_MedicalInstitutionId",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "MedicalInstitutionId",
                table: "Pickups");
        }
    }
}
