using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class CreatePatientImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationImage",
                table: "Patients");

            migrationBuilder.AddColumn<int>(
                name: "PatientImagesId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDPhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SelfiePhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientImages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientImagesId",
                table: "Patients",
                column: "PatientImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_PatientImages_PatientImagesId",
                table: "Patients",
                column: "PatientImagesId",
                principalTable: "PatientImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_PatientImages_PatientImagesId",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "PatientImages");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientImagesId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientImagesId",
                table: "Patients");

            migrationBuilder.AddColumn<byte[]>(
                name: "VerificationImage",
                table: "Patients",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
