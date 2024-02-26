using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SoftlandERPGrafik.Data.Entities.Views
{
    [Table("ZatrudnieniZrodlo")]
    [Keyless]
    public class ZatrudnieniZrodlo
    {
        public string? PRI_Kod { get; set; }

        public string? PRI_Opis { get; set; }

        public string? PRI_Imie1 { get; set; }

        public string? PRI_Nazwisko { get; set; }

        public string? Imie_Nazwisko { get; set; }

        public string? DZL_Kod { get; set; }

        public string? PRE_Plec { get; set; }

        public int? DZL_DzlId { get; set; }

        public int? PRI_PraId { get; set; }
    }
}
