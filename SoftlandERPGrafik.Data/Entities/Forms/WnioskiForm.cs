using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Data.Entities.Forms
{
    public class WnioskiForm : ScheduleEntity
    {
        [Required(ErrorMessage = "Wybór stanowiska pracy jest wymagany")]
        public Guid? RequestId { get; set; }

        [Required(ErrorMessage = "IDS jest wymagany")]
        public string? IDS { get; set; }

        [Required(ErrorMessage = "IDD jest wymagany")]
        public string? IDD { get; set; }

        public int? IloscDni { get; set; }
    }
}
