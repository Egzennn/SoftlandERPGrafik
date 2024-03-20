using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Data.Entities.Forms
{
    public class ScheduleHistoryForm : BaseEntity
    {
        public Guid scheduleId { get; set; }

        public string Column { get; set; }

        public string? Before { get; set; }

        public string? After { get; set; }
    }
}
