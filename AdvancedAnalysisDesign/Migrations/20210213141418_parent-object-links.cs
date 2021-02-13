using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class parentobjectlinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworkTests_PatientBloodworks_PatientBloodworkId",
                table: "PatientBloodworkTests");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedications_Patients_PatientId",
                table: "PatientMedications");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "PatientMedications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientBloodworkId",
                table: "PatientBloodworkTests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientMedicationId",
                table: "PatientBloodworks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks",
                column: "PatientMedicationId",
                principalTable: "PatientMedications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworkTests_PatientBloodworks_PatientBloodworkId",
                table: "PatientBloodworkTests",
                column: "PatientBloodworkId",
                principalTable: "PatientBloodworks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedications_Patients_PatientId",
                table: "PatientMedications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworkTests_PatientBloodworks_PatientBloodworkId",
                table: "PatientBloodworkTests");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedications_Patients_PatientId",
                table: "PatientMedications");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "PatientMedications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PatientBloodworkId",
                table: "PatientBloodworkTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PatientMedicationId",
                table: "PatientBloodworks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks",
                column: "PatientMedicationId",
                principalTable: "PatientMedications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworkTests_PatientBloodworks_PatientBloodworkId",
                table: "PatientBloodworkTests",
                column: "PatientBloodworkId",
                principalTable: "PatientBloodworks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedications_Patients_PatientId",
                table: "PatientMedications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
