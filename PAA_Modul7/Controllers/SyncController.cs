using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Data;
using PAA_Modul7.Services;

namespace PAA_Modul7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly SyncService _syncService;
        private readonly AppDbContext _context;

        public SyncController(SyncService syncService, AppDbContext context)
        {
            _syncService = syncService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Sync()
        {
            var totalData = await _syncService.SyncMahasiswaAsync();

            return Ok(new
            {
                success = true,
                message = "Sinkronisasi data mahasiswa berhasil",
                count = totalData
            });
        }

        [HttpGet("mahasiswa")]
        public async Task<IActionResult> GetMahasiswaLocal()
        {
            var data = await _context.Mahasiswas
                .OrderBy(x => x.Nama)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                data
            });
        }
    }
}
