using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Data.Entities.Forms.Data
{
    public class Holidays
    {
        public string Nazwa { get; set; }

        public string Rodzaj { get; set; }

        [Key]
        public DateTime Data { get; set; }

        public int Rok { get; set; }

        public int Mies { get; set; }

        public int Dzien { get; set; }

        public string DzienTyg { get; set; }
    }
}
