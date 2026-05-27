namespace PAA_Modul7.Models
{
    public class CreatePembayaranRequest
    {
        public int TagihanId { get; set; }
        public decimal JumlahBayar { get; set; }
        public string MetodePembayaran { get; set; } = string.Empty;
        public string Keterangan { get; set; } = string.Empty;
    }
}
