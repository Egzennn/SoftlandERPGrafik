using SoftlandERPGrafik.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoftlandERGrafik.Data.Entities.Forms
{
    public class GrafikForm : BaseEntity
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Required]
        public int LocationId { get; set; }

        public string? Description { get; set; }

        public bool IsAllDay { get; set; }

        public int PRI_PraId { get; set; }

        public int DZL_DzlId { get; set; }

        public Guid? RecurrenceID { get; set; }

        public string? RecurrenceRule { get; set; }

        public string? RecurrenceException { get; set; }
    }
}
