using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PAA_Modul7.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mahasiswas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nama = table.Column<string>(type: "text", nullable: false),
                    ProgramStudi = table.Column<string>(type: "text", nullable: false),
                    MataKuliah = table.Column<string[]>(type: "text[]", nullable: false),
                    StatusAkademik = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    RawDataJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahasiswas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tagihans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MahasiswaId = table.Column<string>(type: "text", nullable: false),
                    Nama = table.Column<string>(type: "text", nullable: false),
                    ProgramStudi = table.Column<string>(type: "text", nullable: false),
                    TahunAkademik = table.Column<string>(type: "text", nullable: false),
                    Semester = table.Column<string>(type: "text", nullable: false),
                    NilaiUkt = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDibayar = table.Column<decimal>(type: "numeric", nullable: false),
                    StatusTagihan = table.Column<string>(type: "text", nullable: false),
                    JatuhTempo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagihans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pembayarans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagihanId = table.Column<int>(type: "integer", nullable: false),
                    MahasiswaId = table.Column<string>(type: "text", nullable: false),
                    Nama = table.Column<string>(type: "text", nullable: false),
                    JumlahBayar = table.Column<decimal>(type: "numeric", nullable: false),
                    MetodePembayaran = table.Column<string>(type: "text", nullable: false),
                    StatusPembayaran = table.Column<string>(type: "text", nullable: false),
                    Keterangan = table.Column<string>(type: "text", nullable: false),
                    TanggalPembayaran = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pembayarans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pembayarans_Tagihans_TagihanId",
                        column: x => x.TagihanId,
                        principalTable: "Tagihans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pembayarans_TagihanId",
                table: "Pembayarans",
                column: "TagihanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mahasiswas");

            migrationBuilder.DropTable(
                name: "Pembayarans");

            migrationBuilder.DropTable(
                name: "Tagihans");
        }
    }
}
