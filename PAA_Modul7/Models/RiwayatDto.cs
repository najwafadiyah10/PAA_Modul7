namespace PAA_Modul7.Models
{
    public class RiwayatDto
    {
        public string MahasiswaId { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;
        public string[] MataKuliah { get; set; } = Array.Empty<string>();
        public string StatusAkademik { get; set; } = string.Empty;
        public List<TagihanEntity> Ukt { get; set; } = new();
        public List<PembayaranEntity> RiwayatBayarUkt { get; set; } = new();
    }
}
