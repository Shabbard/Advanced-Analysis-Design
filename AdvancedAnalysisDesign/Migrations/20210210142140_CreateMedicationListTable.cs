using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class CreateMedicationListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Medications");

            migrationBuilder.AddColumn<int>(
                name: "MedicationListId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicationList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationList", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicationListId",
                table: "Medications",
                column: "MedicationListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationList_MedicationListId",
                table: "Medications",
                column: "MedicationListId",
                principalTable: "MedicationList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationList_MedicationListId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "MedicationList");

            migrationBuilder.DropIndex(
                name: "IX_Medications_MedicationListId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicationListId",
                table: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
