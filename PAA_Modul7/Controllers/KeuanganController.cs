using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Data;
using PAA_Modul7.Models;
using System.Globalization;

namespace PAA_Modul7.Controllers
{
    [ApiController]
    [Route("api/keuangan")]
    public class KeuanganController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KeuanganController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("tagihan")]
        public async Task<IActionResult> CreateTagihan([FromBody] CreateTagihanRequest request)
        {
            var mahasiswa = await _context.Mahasiswas
                .FirstOrDefaultAsync(x => x.Id == request.MahasiswaId);

            if (mahasiswa == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Mahasiswa tidak ditemukan. Jalankan POST /api/Sync terlebih dahulu."
                });
            }

            DateTime? jatuhTempo = null;

            if (!string.IsNullOrWhiteSpace(request.JatuhTempo))
            {
                if (!DateTime.TryParseExact(
                    request.JatuhTempo,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Format jatuhTempo harus dd-MM-yyyy. Contoh: 30-06-2026"
                    });
                }

                jatuhTempo = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            var tagihan = new TagihanEntity
            {
                MahasiswaId = mahasiswa.Id,
                Nama = mahasiswa.Nama,
                ProgramStudi = mahasiswa.ProgramStudi,
                TahunAkademik = request.TahunAkademik,
                Semester = request.Semester,
                NilaiUkt = request.NilaiUkt,
                JatuhTempo = jatuhTempo,
                TotalDibayar = 0,
                StatusTagihan = "belum_lunas",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tagihans.Add(tagihan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Tagihan UKT berhasil dibuat",
                data = tagihan
            });
        }

        [HttpGet("tagihan")]
        public async Task<IActionResult> GetTagihan()
        {
            var data = await _context.Tagihans
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                data
            });
        }

        [HttpGet("tagihan/{mahasiswaId}")]
        public async Task<IActionResult> GetTagihanByMahasiswaId(string mahasiswaId)
        {
            var data = await _context.Tagihans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                message = "Data tagihan mahasiswa berhasil diambil.",
                data
            });
        }

        [HttpPost("pembayaran")]
        public async Task<IActionResult> CreatePembayaran([FromBody] CreatePembayaranRequest request)
        {
            var tagihan = await _context.Tagihans
                .FirstOrDefaultAsync(x => x.Id == request.TagihanId);

            if (tagihan == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Tagihan tidak ditemukan"
                });
            }

            if (request.JumlahBayar <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Jumlah bayar harus lebih dari 0"
                });
            }

            if (tagihan.StatusTagihan == "lunas")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Tagihan sudah lunas"
                });
            }

            var pembayaran = new PembayaranEntity
            {
                TagihanId = tagihan.Id,
                MahasiswaId = tagihan.MahasiswaId,
                Nama = tagihan.Nama,
                JumlahBayar = request.JumlahBayar,
                MetodePembayaran = request.MetodePembayaran,
                Keterangan = request.Keterangan,
                StatusPembayaran = "berhasil",
                TanggalPembayaran = DateTime.UtcNow
            };

            tagihan.TotalDibayar += request.JumlahBayar;
            tagihan.UpdatedAt = DateTime.UtcNow;

            if (tagihan.TotalDibayar >= tagihan.NilaiUkt)
            {
                tagihan.StatusTagihan = "lunas";
            }
            else if (tagihan.TotalDibayar > 0)
            {
                tagihan.StatusTagihan = "sebagian";
            }
            else
            {
                tagihan.StatusTagihan = "belum_lunas";
            }

            _context.Pembayarans.Add(pembayaran);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Pembayaran berhasil disimpan",
                data = new
                {
                    pembayaran,
                    tagihan
                }
            });
        }

        [HttpGet("pembayaran/{mahasiswaId}")]
        public async Task<IActionResult> GetPembayaranByMahasiswaId(string mahasiswaId)
        {
            var data = await _context.Pembayarans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.TanggalPembayaran)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                message = "Riwayat pembayaran mahasiswa berhasil diambil.",
                data
            });
        }

        [HttpGet("mahasiswa/{mahasiswaId}")]
        public async Task<IActionResult> GetKeuanganMahasiswa(string mahasiswaId)
        {
            var mahasiswa = await _context.Mahasiswas
                .FirstOrDefaultAsync(x => x.Id == mahasiswaId);

            if (mahasiswa == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Mahasiswa tidak ditemukan"
                });
            }

            var tagihan = await _context.Tagihans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var pembayaran = await _context.Pembayarans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.TanggalPembayaran)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Data keuangan mahasiswa berhasil diambil.",
                data = new CombinedKeuanganDto
                {
                    MahasiswaId = mahasiswa.Id,
                    Nama = mahasiswa.Nama,
                    ProgramStudi = mahasiswa.ProgramStudi,
                    MataKuliah = mahasiswa.MataKuliah,
                    StatusAkademik = mahasiswa.StatusAkademik,
                    Tagihan = tagihan,
                    RiwayatPembayaran = pembayaran
                }
            });
        }
    }
}
