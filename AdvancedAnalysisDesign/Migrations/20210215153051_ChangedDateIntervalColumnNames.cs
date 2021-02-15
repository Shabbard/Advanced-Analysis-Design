using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class ChangedDateIntervalColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications");

            migrationBuilder.DropColumn(
                name: "DateIntervalOfPickup",
                table: "PatientMedications");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateScheduled",
                table: "Pickups",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DayIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DayIntervalOfPickup",
                table: "PatientMedications",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayIntervalOfBloodworkRenewal",
                table: "PatientMedications");

            migrationBuilder.DropColumn(
                name: "DayIntervalOfPickup",
                table: "PatientMedications");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateScheduled",
                table: "Pickups",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfBloodworkRenewal",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateIntervalOfPickup",
                table: "PatientMedications",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
