using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class fixestocountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryConnector",
                table: "CountryConnector");

            migrationBuilder.RenameTable(
                name: "CountryConnector",
                newName: "CountryConnectors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryConnectors",
                table: "CountryConnectors",
                column: "CountryConnectorId");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryConnectors",
                table: "CountryConnectors");

            migrationBuilder.RenameTable(
                name: "CountryConnectors",
                newName: "CountryConnector");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryConnector",
                table: "CountryConnector",
                column: "CountryConnectorId");
        }
    }
}
