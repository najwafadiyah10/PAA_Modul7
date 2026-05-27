using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAA_Modul7.Models
{
    public class PembayaranEntity
    {
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("uktId")]
        public int TagihanId { get; set; }

        [JsonIgnore]
        public TagihanEntity? Tagihan { get; set; }

        public string MahasiswaId { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;

        public decimal JumlahBayar { get; set; }

        [JsonPropertyName("metodeBayarUkt")]
        public string MetodePembayaran { get; set; } = string.Empty;

        [JsonPropertyName("statusBayarUkt")]
        public string StatusPembayaran { get; set; } = "berhasil";

        public string Keterangan { get; set; } = string.Empty;

        [JsonPropertyName("tanggalBayarUkt")]
        public DateTime TanggalPembayaran { get; set; } = DateTime.UtcNow;
    }
}
