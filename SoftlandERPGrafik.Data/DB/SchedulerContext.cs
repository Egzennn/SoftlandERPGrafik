using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftlandERPGrafik.Data.DB
{
    public class SchedulerContext : DbContext
    {
        public SchedulerContext(DbContextOptions<SchedulerContext> options)
            : base(options)
        {
        }

        public DbSet<GrafikForm> GrafikForms { get; set; }

        public DbSet<WnioskiForm> WnioskiForms { get; set; }

    }
}
