using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class moviesandcountriesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryConnector",
                columns: table => new
                {
                    CountryConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryConnector", x => x.CountryConnectorId);
                });

            migrationBuilder.CreateTable(
                name: "GenreConnectors",
                columns: table => new
                {
                    GenreConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreConnectors", x => x.GenreConnectorId);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgeRestriction = table.Column<int>(type: "int", nullable: false),
                    CinemaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryConnector");

            migrationBuilder.DropTable(
                name: "GenreConnectors");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
