using Microsoft.EntityFrameworkCore.Migrations;

namespace Servinte.Framework.Clinic.BasicInformation.API.Migrations
{
    public partial class MigrationNameV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrupoSanguineo",
                table: "Patients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoSanguineo",
                table: "Patients");
        }
    }
}
