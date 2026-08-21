using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartupEmpire.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankingEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NetWorth = table.Column<double>(type: "double precision", nullable: false),
                    Valuation = table.Column<double>(type: "double precision", nullable: false),
                    MonthlyRecurringRevenue = table.Column<double>(type: "double precision", nullable: false),
                    ProgressStageIndex = table.Column<int>(type: "integer", nullable: false),
                    AchievementCount = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    OwnerPlayerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ReferralRedemptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    InviterPlayerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    InviteePlayerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RedeemedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRedemptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_PlayerId",
                table: "RankingEntries",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralCodes_OwnerPlayerId",
                table: "ReferralCodes",
                column: "OwnerPlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRedemptions_InviteePlayerId",
                table: "ReferralRedemptions",
                column: "InviteePlayerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingEntries");

            migrationBuilder.DropTable(
                name: "ReferralCodes");

            migrationBuilder.DropTable(
                name: "ReferralRedemptions");
        }
    }
}
