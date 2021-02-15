using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class ChangeTimeSpanToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DateIntervalOfPickup",
                table: "PatientMedications",
                type: "float",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<double>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "float",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DateIntervalOfPickup",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
