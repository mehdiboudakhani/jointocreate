using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Hubs",
                columns: table => new
                {
                    VoiceChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChildName = table.Column<string>(type: "TEXT", nullable: false),
                    UserLimit = table.Column<int>(type: "INTEGER", nullable: false),
                    TemporaryVoiceChannelInterface = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hubs", x => x.VoiceChannelId);
                    table.ForeignKey(
                        name: "FK_Hubs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemporaryVoiceChannels",
                columns: table => new
                {
                    VoiceChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HubVoiceChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporaryVoiceChannels", x => x.VoiceChannelId);
                    table.ForeignKey(
                        name: "FK_TemporaryVoiceChannels_Hubs_HubVoiceChannelId",
                        column: x => x.HubVoiceChannelId,
                        principalTable: "Hubs",
                        principalColumn: "VoiceChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hubs_GuildId",
                table: "Hubs",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryVoiceChannels_HubVoiceChannelId",
                table: "TemporaryVoiceChannels",
                column: "HubVoiceChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporaryVoiceChannels");

            migrationBuilder.DropTable(
                name: "Hubs");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
