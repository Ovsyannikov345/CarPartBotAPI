using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPartBotApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommandStateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionStep",
                table: "UserInteractionStates");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "UserInteractionStates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionStep",
                table: "UserInteractionStates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "UserInteractionStates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
