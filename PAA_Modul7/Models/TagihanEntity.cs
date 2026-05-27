using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAA_Modul7.Models
{
    public class TagihanEntity
    {
        [Key]
        public int Id { get; set; }

        public string MahasiswaId { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;

        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public decimal NilaiUkt { get; set; }
        public decimal TotalDibayar { get; set; } = 0;

        [JsonPropertyName("statusUkt")]
        public string StatusTagihan { get; set; } = "belum_lunas";

        public DateTime? JatuhTempo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
