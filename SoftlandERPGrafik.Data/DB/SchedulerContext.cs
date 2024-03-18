using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Data.Entities.Forms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Data.DB
{
    public class ScheduleContext : DbContext
    {
        public ScheduleContext(DbContextOptions<ScheduleContext> options)
            : base(options)
        {
        }

        public DbSet<ScheduleForm> ScheduleForms { get; set; }

        //public DbSet<Holidays> HolidaysData { get; set; }
    }
}
