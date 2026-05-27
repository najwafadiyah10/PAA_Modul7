using System.ComponentModel.DataAnnotations;

namespace PAA_Modul7.Models
{
    public class MahasiswaEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string Nama { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;
        public string[] MataKuliah { get; set; } = Array.Empty<string>();
        public string StatusAkademik { get; set; } = string.Empty;
        public int Version { get; set; }
        public string RawDataJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
    }
}
