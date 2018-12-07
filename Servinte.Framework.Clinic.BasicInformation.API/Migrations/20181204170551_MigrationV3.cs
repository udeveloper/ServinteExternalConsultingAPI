using Microsoft.EntityFrameworkCore.Migrations;

namespace Servinte.Framework.Clinic.BasicInformation.API.Migrations
{
    public partial class MigrationV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationPublishers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Applicacion = table.Column<string>(nullable: true),
                    Module = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    ExchageName = table.Column<string>(nullable: true),
                    ExchangeType = table.Column<string>(nullable: true),
                    QueueName = table.Column<string>(nullable: true),
                    KeyRouting = table.Column<string>(nullable: true),
                    KeyBinding = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationPublishers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationPublishers");
        }
    }
}
