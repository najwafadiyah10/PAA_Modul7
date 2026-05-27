namespace PAA_Modul7.Models
{
    public class MahasiswaApiResponse
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public List<MahasiswaEntity> Data { get; set; } = new();
    }
}
