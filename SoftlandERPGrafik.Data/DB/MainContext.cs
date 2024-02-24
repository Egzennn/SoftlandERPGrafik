using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Views;

namespace SoftlandERPGrafik.Data.DB
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
        }

        public DbSet<OrganizacjaLokalizacje> OrganizacjaLokalizacjeVocabulary { get; set; }

        public DbSet<ZatrudnieniDzialy> ZatrudnieniDzialy { get; set; }

        public DbSet<ZatrudnieniZrodlo> ZatrudnieniZrodlo { get; set; }

        public DbSet<Kierownicy> Kierownicy { get; set; }
    }
}