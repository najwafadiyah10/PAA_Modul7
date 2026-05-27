namespace PAA_Modul7.Models
{
    public class CreateBayarUktRequest
    {
        public int UktId { get; set; }
        public decimal JumlahBayar { get; set; }
        public string MetodeBayarUkt { get; set; } = string.Empty;
        public string Keterangan { get; set; } = string.Empty;
    }
}
