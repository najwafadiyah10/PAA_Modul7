using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Models;

namespace PAA_Modul7.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<MahasiswaEntity> Mahasiswas => Set<MahasiswaEntity>();
        public DbSet<TagihanEntity> Tagihans => Set<TagihanEntity>();
        public DbSet<PembayaranEntity> Pembayarans => Set<PembayaranEntity>();
    }
}
