using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastApiWebhook.Migrations
{
    /// <inheritdoc />
    public partial class AddedMovieDateTimeAsAnString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateTime",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Movies");
        }
    }
}
