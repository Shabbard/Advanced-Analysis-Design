using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class EditTablesToMatchSchema_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodworkTests");

            migrationBuilder.DropTable(
                name: "Bloodworks");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateTable(
                name: "PatientBloodworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfResults = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PatientMedicationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientBloodworks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientBloodworks_PatientMedications_PatientMedicationId",
                        column: x => x.PatientMedicationId,
                        principalTable: "PatientMedications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientBloodworkTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    PatientBloodworkId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientBloodworkTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientBloodworkTests_PatientBloodworks_PatientBloodworkId",
                        column: x => x.PatientBloodworkId,
                        principalTable: "PatientBloodworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodworks_PatientMedicationId",
                table: "PatientBloodworks",
                column: "PatientMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodworkTests_PatientBloodworkId",
                table: "PatientBloodworkTests",
                column: "PatientBloodworkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientBloodworkTests");

            migrationBuilder.DropTable(
                name: "PatientBloodworks");

            migrationBuilder.DropColumn(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications");

            migrationBuilder.CreateTable(
                name: "Bloodworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfResults = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PatientMedicationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloodworks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bloodworks_PatientMedications_PatientMedicationId",
                        column: x => x.PatientMedicationId,
                        principalTable: "PatientMedications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BloodworkTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodworkId = table.Column<int>(type: "int", nullable: true),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    DateOfTest = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodworkTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodworkTests_Bloodworks_BloodworkId",
                        column: x => x.BloodworkId,
                        principalTable: "Bloodworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bloodworks_PatientMedicationId",
                table: "Bloodworks",
                column: "PatientMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodworkTests_BloodworkId",
                table: "BloodworkTests",
                column: "BloodworkId");
        }
    }
}
