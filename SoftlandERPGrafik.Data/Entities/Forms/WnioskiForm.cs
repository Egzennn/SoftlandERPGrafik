using System.ComponentModel.DataAnnotations;

namespace SoftlandERPGrafik.Data.Entities.Forms
{
    public class WnioskiForm : ScheduleEntity
    {
        [Required(ErrorMessage = "Wybór stanowiska pracy jest wymagany")]
        public Guid RequestId { get; set; }

        [Required(ErrorMessage = "IDS jest wymagany")]
        public string? IDS { get; set; }

        [Required(ErrorMessage = "IDD jest wymagany")]
        public string? IDD { get; set; }

        public int? IloscDni { get; set; }
    }
}
