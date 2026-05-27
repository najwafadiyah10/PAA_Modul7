namespace PAA_Modul7.Models
{
    public class CreateTagihanRequest
    {
        public string MahasiswaId { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public decimal NilaiUkt { get; set; }

        // Format input dari Swagger/Postman: dd-MM-yyyy, contoh 30-06-2026.
        public string? JatuhTempo { get; set; }
    }
}
