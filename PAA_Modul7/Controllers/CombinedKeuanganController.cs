using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Data;
using PAA_Modul7.Models;

namespace PAA_Modul7.Controllers
{
    [ApiController]
    [Route("api/combined-keuangan")]
    public class CombinedKeuanganController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CombinedKeuanganController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var mahasiswas = await _context.Mahasiswas
                .OrderBy(x => x.Nama)
                .ToListAsync();

            var result = new List<CombinedKeuanganDto>();

            foreach (var mahasiswa in mahasiswas)
            {
                var tagihan = await _context.Tagihans
                    .Where(x => x.MahasiswaId == mahasiswa.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var pembayaran = await _context.Pembayarans
                    .Where(x => x.MahasiswaId == mahasiswa.Id)
                    .OrderByDescending(x => x.TanggalPembayaran)
                    .ToListAsync();

                result.Add(new CombinedKeuanganDto
                {
                    MahasiswaId = mahasiswa.Id,
                    Nama = mahasiswa.Nama,
                    ProgramStudi = mahasiswa.ProgramStudi,
                    MataKuliah = mahasiswa.MataKuliah,
                    StatusAkademik = mahasiswa.StatusAkademik,
                    Tagihan = tagihan,
                    RiwayatPembayaran = pembayaran
                });
            }

            return Ok(new
            {
                success = true,
                count = result.Count,
                data = result
            });
        }
    }
}
