namespace PAA_Modul7.Models
{
    public class CreateUktRequest
    {
        public string MahasiswaId { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public decimal NilaiUkt { get; set; }
        public string? JatuhTempo { get; set; }
    }
}
