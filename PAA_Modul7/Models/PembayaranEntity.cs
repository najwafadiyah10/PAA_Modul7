using System.ComponentModel.DataAnnotations;

namespace PAA_Modul7.Models
{
    public class PembayaranEntity
    {
        [Key]
        public int Id { get; set; }

        public int TagihanId { get; set; }
        public TagihanEntity? Tagihan { get; set; }

        public string MahasiswaId { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;

        public decimal JumlahBayar { get; set; }
        public string MetodePembayaran { get; set; } = string.Empty;
        public string StatusPembayaran { get; set; } = "berhasil";
        public string Keterangan { get; set; } = string.Empty;
        public DateTime TanggalPembayaran { get; set; } = DateTime.UtcNow;
    }
}
