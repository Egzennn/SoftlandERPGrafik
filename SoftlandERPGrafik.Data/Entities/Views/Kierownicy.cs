using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftlandERPGrafik.Data.Entities.Views
{
    [Table("Kierownicy")]
    [Keyless]
    public class Kierownicy
    {
        public string PRI_Imie1 { get; set; }

        public string PRI_Nazwisko { get; set; }

        public string PRI_Opis { get; set; }

        public int PRI_PraId { get; set; }

        public string CNT_Nazwa { get; set; }

    }
}
