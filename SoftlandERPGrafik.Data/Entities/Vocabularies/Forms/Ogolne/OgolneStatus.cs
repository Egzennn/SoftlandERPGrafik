using System.ComponentModel;

namespace SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne
{
    public class OgolneStatus : BaseEntity
    {
        [DisplayName("Wartość")]
        public string? Wartosc { get; set; }

        [DisplayName("Odpowiedzialny")]
        public string? Odpowiedzialny { get; set; }
    }
}