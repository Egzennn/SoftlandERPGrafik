using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftlandERPGrafik.Data.Entities.Views
{
    [Table("ZatrudnieniDzialy")]
    [Keyless]
    public class ZatrudnieniDzialy
    {
        public int DZL_DzlId { get; set; }

        public string? DZL_Kod { get; set; }
    }
}
