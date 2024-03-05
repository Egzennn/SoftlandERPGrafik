using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftlandERPGrafik.Data.Entities.Forms
{
    public class GrafikForm : ScheduleEntity
    {

        [Required(ErrorMessage = "Wybór stanowiska pracy jest wymagany")]
        public int? LocationId { get; set; }
    }
}
