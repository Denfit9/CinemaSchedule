using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class Cinemaadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    CinemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CinemaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CinemaDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CinemaAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DirectorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CinemaPicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.CinemaId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cinemas");
        }
    }
}
