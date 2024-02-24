using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Views;

namespace SoftlandERPGrafik.Data.DB
{
    public class OptimaContext : DbContext
    {
        public OptimaContext(DbContextOptions<OptimaContext> options)
            : base(options)
        {
        }
    }
}