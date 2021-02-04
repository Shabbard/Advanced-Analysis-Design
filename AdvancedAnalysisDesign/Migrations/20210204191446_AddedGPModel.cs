using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class AddedGPModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GPId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GP_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_GPId",
                table: "Patients",
                column: "GPId");

            migrationBuilder.CreateIndex(
                name: "IX_GP_UserId",
                table: "GP",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_GP_GPId",
                table: "Patients",
                column: "GPId",
                principalTable: "GP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_GP_GPId",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "GP");

            migrationBuilder.DropIndex(
                name: "IX_Patients_GPId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "GPId",
                table: "Patients");
        }
    }
}
