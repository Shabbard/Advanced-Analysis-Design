using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class CreateMedicationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Pickups_PickupId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_Medications_MedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropIndex(
                name: "IX_Medications_PatientId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_PickupId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "BloodworkRequired",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DateIntervalOfBloodworkRenewal",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "PatientId",
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

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Medications",
                newName: "MedicationName");

            migrationBuilder.CreateTable(
                name: "PatientMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: true),
                    BloodworkRequired = table.Column<bool>(type: "bit", nullable: false),
                    DateIntervalOfBloodworkRenewal = table.Column<TimeSpan>(type: "time", nullable: false),
                    PickupId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Pickups_PickupId",
                        column: x => x.PickupId,
                        principalTable: "Pickups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_MedicationId",
                table: "PatientMedications",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_PatientId",
                table: "PatientMedications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_PickupId",
                table: "PatientMedications",
                column: "PickupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks",
                column: "PatientMedicationId",
                principalTable: "PatientMedications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                table: "PatientBloodworks");

            migrationBuilder.DropTable(
                name: "PatientMedications");

            migrationBuilder.RenameColumn(
                name: "PatientMedicationId",
                table: "PatientBloodworks",
                newName: "MedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientBloodworks_PatientMedicationId",
                table: "PatientBloodworks",
                newName: "IX_PatientBloodworks_MedicationId");

            migrationBuilder.RenameColumn(
                name: "MedicationName",
                table: "Medications",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "BloodworkRequired",
                table: "Medications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "Medications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PickupId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medications_PatientId",
                table: "Medications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_PickupId",
                table: "Medications",
                column: "PickupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
