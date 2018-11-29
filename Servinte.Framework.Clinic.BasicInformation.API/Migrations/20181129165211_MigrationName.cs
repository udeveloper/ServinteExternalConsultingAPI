using Microsoft.EntityFrameworkCore.Migrations;

namespace Servinte.Framework.Clinic.BasicInformation.API.Migrations
{
    public partial class MigrationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(nullable: true),
                    TipoIdentificacion = table.Column<string>(nullable: true),
                    NumeroIdentificacion = table.Column<long>(nullable: false),
                    Edad = table.Column<int>(nullable: false),
                    Peso = table.Column<decimal>(nullable: false),
                    MasaCorporal = table.Column<decimal>(nullable: false),
                    SuperficieCorporal = table.Column<decimal>(nullable: false),
                    Genero = table.Column<string>(nullable: true),
                    Identificador = table.Column<long>(nullable: false),
                    Talla = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
