using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class Addedhalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HallName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HallDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CinemaId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Halls");
        }
    }
}
