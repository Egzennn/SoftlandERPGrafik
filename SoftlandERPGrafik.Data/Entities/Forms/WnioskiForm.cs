using System.ComponentModel.DataAnnotations;

namespace SoftlandERPGrafik.Data.Entities.Forms
{
    public class WnioskiForm : ScheduleEntity
    {
        [Required(ErrorMessage = "Wybór stanowiska pracy jest wymagany")]
        public Guid RequestId { get; set; }

        public string? IDS { get; set; }

        public string? IDD { get; set; }

        public int? IloscDni { get; set; }
    }
}
