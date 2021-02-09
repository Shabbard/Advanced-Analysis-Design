using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class CreatePickupTableAndModifyPatientMedication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropColumn(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications");

            migrationBuilder.RenameColumn(
                name: "PatientMedicationId",
                table: "PatientBloodworks",
                newName: "MedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientBloodworks_PatientMedicationId",
                table: "PatientBloodworks",
                newName: "IX_PatientBloodworks_MedicationId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "Medications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "PickupId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pickups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPickedUp = table.Column<bool>(type: "bit", nullable: false),
                    IsPrepared = table.Column<bool>(type: "bit", nullable: false),
                    DatePickedUp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_PickupId",
                table: "Medications",
                column: "PickupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Pickups_PickupId",
                table: "Medications",
                column: "PickupId",
                principalTable: "Pickups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_Medications_MedicationId",
                table: "PatientBloodworks",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Pickups_PickupId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_Medications_MedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropTable(
                name: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_Medications_PickupId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DateIntervalOfBloodworkRenewal",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "PickupId",
                table: "Medications");

            migrationBuilder.RenameColumn(
                name: "MedicationId",
                table: "PatientBloodworks",
                newName: "PatientMedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientBloodworks_MedicationId",
                table: "PatientBloodworks",
                newName: "IX_PatientBloodworks_PatientMedicationId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks",
                column: "PatientMedicationId",
                principalTable: "PatientMedications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
