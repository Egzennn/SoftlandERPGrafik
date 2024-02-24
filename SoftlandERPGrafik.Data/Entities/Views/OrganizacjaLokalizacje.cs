using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftlandERPGrafik.Data.Entities.Views
{
    [Table("OrganizacjaLokalizacjeVocabulary")]
    [Keyless]
    public class OrganizacjaLokalizacje
    {
        public string? Wartosc {  get; set; }
        public int Lok_LokId { get; set; }
    }
}
