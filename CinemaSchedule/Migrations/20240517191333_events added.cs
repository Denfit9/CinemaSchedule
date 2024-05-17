using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class eventsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HallId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Begins = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ends = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
