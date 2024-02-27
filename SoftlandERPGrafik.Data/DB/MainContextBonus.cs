using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;

namespace SoftlandERPGrafik.Data.DB
{
    public class MainContextBonus : DbContext
    {
        public MainContextBonus(DbContextOptions<MainContextBonus> options)
            : base(options)
        {
        }

        public DbSet<OgolneStan> OgolneStanVocabulary { get; set; }

        public DbSet<OrganizacjaLokalizacje> OrganizacjaLokalizacjeVocabulary { get; set; }

        public DbSet<ZatrudnieniDzialy> ZatrudnieniDzialy { get; set; }

        public DbSet<ZatrudnieniZrodlo> ZatrudnieniZrodlo { get; set; }

        public DbSet<Kierownicy> Kierownicy { get; set; }
    }
}