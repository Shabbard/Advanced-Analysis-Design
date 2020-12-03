using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedAnalysisDesign.Migrations
{
    public partial class changeusernametoemail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "UserDetails");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "EmailAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
