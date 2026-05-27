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

        /// <summary>
        /// Membuat data UKT mahasiswa.
        /// </summary>
        [HttpPost("ukt")]
        public async Task<IActionResult> CreateUkt([FromBody] CreateUktRequest request)
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

            var ukt = new TagihanEntity
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

            _context.Tagihans.Add(ukt);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "UKT berhasil dibuat",
                data = ukt
            });
        }

        /// <summary>
        /// Menampilkan semua data UKT.
        /// </summary>
        [HttpGet("ukt")]
        public async Task<IActionResult> GetUkt()
        {
            var data = await _context.Tagihans
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                message = "Data UKT berhasil diambil.",
                data
            });
        }

        /// <summary>
        /// Menampilkan data UKT berdasarkan id mahasiswa.
        /// </summary>
        [HttpGet("ukt/{mahasiswaId}")]
        public async Task<IActionResult> GetUktByMahasiswaId(string mahasiswaId)
        {
            var data = await _context.Tagihans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                message = "Data UKT mahasiswa berhasil diambil.",
                data
            });
        }

        /// <summary>
        /// Melakukan pembayaran UKT mahasiswa.
        /// </summary>
        [HttpPost("bayar-ukt")]
        public async Task<IActionResult> BayarUkt([FromBody] CreateBayarUktRequest request)
        {
            var ukt = await _context.Tagihans
                .FirstOrDefaultAsync(x => x.Id == request.UktId);

            if (ukt == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Data UKT tidak ditemukan"
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

            if (ukt.StatusTagihan == "lunas")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "UKT sudah lunas"
                });
            }

            var bayarUkt = new PembayaranEntity
            {
                TagihanId = ukt.Id,
                MahasiswaId = ukt.MahasiswaId,
                Nama = ukt.Nama,
                JumlahBayar = request.JumlahBayar,
                MetodePembayaran = request.MetodeBayarUkt,
                Keterangan = request.Keterangan,
                StatusPembayaran = "berhasil",
                TanggalPembayaran = DateTime.UtcNow
            };

            ukt.TotalDibayar += request.JumlahBayar;
            ukt.UpdatedAt = DateTime.UtcNow;

            if (ukt.TotalDibayar >= ukt.NilaiUkt)
            {
                ukt.StatusTagihan = "lunas";
            }
            else if (ukt.TotalDibayar > 0)
            {
                ukt.StatusTagihan = "sebagian";
            }
            else
            {
                ukt.StatusTagihan = "belum_lunas";
            }

            _context.Pembayarans.Add(bayarUkt);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Bayar UKT berhasil disimpan",
                data = new
                {
                    bayarUkt,
                    ukt
                }
            });
        }

        /// <summary>
        /// Menampilkan riwayat bayar UKT berdasarkan id mahasiswa.
        /// </summary>
        [HttpGet("riwayat/{mahasiswaId}")]
        public async Task<IActionResult> GetRiwayatBayarUktByMahasiswaId(string mahasiswaId)
        {
            var data = await _context.Pembayarans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.TanggalPembayaran)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                message = "Riwayat bayar UKT mahasiswa berhasil diambil.",
                data
            });
        }

        /// <summary>
        /// Menampilkan riwayat keuangan satu mahasiswa.
        /// </summary>
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

            var ukt = await _context.Tagihans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var bayarUkt = await _context.Pembayarans
                .Where(x => x.MahasiswaId == mahasiswaId)
                .OrderByDescending(x => x.TanggalPembayaran)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Riwayat keuangan mahasiswa berhasil diambil.",
                data = new RiwayatDto
                {
                    MahasiswaId = mahasiswa.Id,
                    Nama = mahasiswa.Nama,
                    ProgramStudi = mahasiswa.ProgramStudi,
                    MataKuliah = mahasiswa.MataKuliah,
                    StatusAkademik = mahasiswa.StatusAkademik,
                    Ukt = ukt,
                    RiwayatBayarUkt = bayarUkt
                }
            });
        }
    }
}