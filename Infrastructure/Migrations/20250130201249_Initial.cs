using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Criminals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CriminalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Offenses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WatchlistStatus = table.Column<bool>(type: "bit", nullable: false),
                    ArrestDates = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criminals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CriminalBiometrics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CriminalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FingerprintData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DNAProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriminalBiometrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriminalBiometrics_Criminals_CriminalId",
                        column: x => x.CriminalId,
                        principalTable: "Criminals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CriminalPictures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CriminalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Mugshot = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    AdditionalPictures = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriminalPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriminalPictures_Criminals_CriminalId",
                        column: x => x.CriminalId,
                        principalTable: "Criminals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriminalBiometrics_CriminalId",
                table: "CriminalBiometrics",
                column: "CriminalId");

            migrationBuilder.CreateIndex(
                name: "IX_CriminalPictures_CriminalId",
                table: "CriminalPictures",
                column: "CriminalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriminalBiometrics");

            migrationBuilder.DropTable(
                name: "CriminalPictures");

            migrationBuilder.DropTable(
                name: "Criminals");
        }
    }
}
