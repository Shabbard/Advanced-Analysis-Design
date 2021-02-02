using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class BloodworkUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "PatientBloodworkTests");

            migrationBuilder.RenameColumn(
                name: "Results",
                table: "PatientBloodworkTests",
                newName: "TestType");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "PatientBloodworkTests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "PatientBloodworkTests");

            migrationBuilder.RenameColumn(
                name: "TestType",
                table: "PatientBloodworkTests",
                newName: "Results");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "PatientBloodworkTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
