using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSchedule.Migrations
{
    /// <inheritdoc />
    public partial class eventsaddedandfixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieId",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Events");
        }
    }
}
