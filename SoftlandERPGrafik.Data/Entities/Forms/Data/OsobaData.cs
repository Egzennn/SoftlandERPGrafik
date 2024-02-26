using SoftlandERPGrafik.Data.Entities.Views;

namespace SoftlandERPGrafik.Data.Entities.Forms.Data
{
    public class OsobaData
    {
        public string Imie_Nazwisko { get; set; }

        public int PRI_PraId { get; set; }

        public int DZL_DzlId { get; set; }

        public string DZL_Kod { get; set; }

        public string PRI_Opis { get; set; }

        public OsobaData(ZatrudnieniZrodlo zatrudniony)
        {
            this.Imie_Nazwisko = zatrudniony.Imie_Nazwisko ?? string.Empty;
            this.PRI_PraId = zatrudniony.PRI_PraId ?? 0;
            this.DZL_DzlId = zatrudniony.DZL_DzlId ?? 0;
            this.DZL_Kod = zatrudniony.DZL_Kod ?? string.Empty;
            this.PRI_Opis = zatrudniony.PRI_Opis ?? string.Empty;
        }
    }
}
