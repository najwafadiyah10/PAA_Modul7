using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Data;
using PAA_Modul7.Models;

namespace PAA_Modul7.Controllers
{
    [ApiController]
    [Route("api/riwayat")]
    public class RiwayatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiwayatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRiwayat()
        {
            var mahasiswas = await _context.Mahasiswas
                .OrderBy(x => x.Nama)
                .ToListAsync();

            var result = new List<RiwayatDto>();

            foreach (var mahasiswa in mahasiswas)
            {
                var ukt = await _context.Tagihans
                    .Where(x => x.MahasiswaId == mahasiswa.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var bayarUkt = await _context.Pembayarans
                    .Where(x => x.MahasiswaId == mahasiswa.Id)
                    .OrderByDescending(x => x.TanggalPembayaran)
                    .ToListAsync();

                result.Add(new RiwayatDto
                {
                    MahasiswaId = mahasiswa.Id,
                    Nama = mahasiswa.Nama,
                    ProgramStudi = mahasiswa.ProgramStudi,
                    MataKuliah = mahasiswa.MataKuliah,
                    StatusAkademik = mahasiswa.StatusAkademik,
                    Ukt = ukt,
                    RiwayatBayarUkt = bayarUkt
                });
            }

            return Ok(new
            {
                success = true,
                count = result.Count,
                message = "Riwayat mahasiswa, UKT, dan bayar UKT berhasil diambil.",
                data = result
            });
        }
    }
}
