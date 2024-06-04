using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicTakToe.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    FinishedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinancialMovment = table.Column<double>(type: "double precision", nullable: false),
                    IsFreeGame = table.Column<bool>(type: "boolean", nullable: false),
                    IsWin = table.Column<bool>(type: "boolean", nullable: false),
                    OponentName = table.Column<string>(type: "text", nullable: true),
                    OponentPhoto = table.Column<string>(type: "text", nullable: true),
                    StatusGame = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTg = table.Column<int>(type: "integer", nullable: true),
                    Is_bot = table.Column<bool>(type: "boolean", nullable: true),
                    First_name = table.Column<string>(type: "text", nullable: true),
                    Last_name = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Language_code = table.Column<string>(type: "text", nullable: true),
                    Is_premium = table.Column<bool>(type: "boolean", nullable: true),
                    Added_to_attachment_menu = table.Column<bool>(type: "boolean", nullable: true),
                    Allows_write_to_pm = table.Column<bool>(type: "boolean", nullable: true),
                    Photo_url = table.Column<string>(type: "text", nullable: true),
                    FreeCoin = table.Column<double>(type: "double precision", nullable: true),
                    TonCoin = table.Column<double>(type: "double precision", nullable: true),
                    SeenChanges = table.Column<bool>(type: "boolean", nullable: false),
                    LastGiftReceived = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ParticipantCreater = table.Column<int>(type: "integer", nullable: false),
                    ParticipantJoined = table.Column<int>(type: "integer", nullable: true),
                    IsFreeGame = table.Column<bool>(type: "boolean", nullable: false),
                    TableId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastMoveFrom = table.Column<int>(type: "integer", nullable: false),
                    LastMoveFromSee = table.Column<bool>(type: "boolean", nullable: false),
                    LastMoveDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastMoveIndex = table.Column<int>(type: "integer", nullable: true),
                    MoveTime = table.Column<int>(type: "integer", nullable: false),
                    IsPrivateGame = table.Column<bool>(type: "boolean", nullable: false),
                    PriceGame = table.Column<double>(type: "double precision", nullable: false),
                    WithBot = table.Column<bool>(type: "boolean", nullable: false),
                    FirstStep = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    GameId1 = table.Column<int>(type: "integer", nullable: true),
                    WallSize = table.Column<int>(type: "integer", nullable: false),
                    WinSize = table.Column<int>(type: "integer", nullable: false),
                    Cells = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Games_GameId1",
                        column: x => x.GameId1,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_TableId",
                table: "Games",
                column: "TableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_GameId1",
                table: "Tables",
                column: "GameId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Tables_TableId",
                table: "Games",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Tables_TableId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
