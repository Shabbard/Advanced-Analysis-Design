using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class bloodwork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfResults",
                table: "PatientBloodworks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfUpload",
                table: "PatientBloodworkTests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "BloodworkTestId",
                table: "PatientBloodworks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodworks_BloodworkTestId",
                table: "PatientBloodworks",
                column: "BloodworkTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodworks_BloodworkTests_BloodworkTestId",
                table: "PatientBloodworks",
                column: "BloodworkTestId",
                principalTable: "BloodworkTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodworks_BloodworkTests_BloodworkTestId",
                table: "PatientBloodworks");

            migrationBuilder.DropIndex(
                name: "IX_PatientBloodworks_BloodworkTestId",
                table: "PatientBloodworks");

            migrationBuilder.DropColumn(
                name: "DateOfUpload",
                table: "PatientBloodworkTests");

            migrationBuilder.DropColumn(
                name: "BloodworkTestId",
                table: "PatientBloodworks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfResults",
                table: "PatientBloodworks",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
