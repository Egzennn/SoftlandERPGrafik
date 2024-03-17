using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;

namespace SoftlandERPGrafik.Data.DB
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
        }

        public DbSet<GrafikForm> GrafikForms { get; set; }

        public DbSet<WnioskiForm> WnioskiForms { get; set; }

        public DbSet<ScheduleForm> ScheduleForms { get; set; }

        public DbSet<OgolneStan> OgolneStanVocabulary { get; set; }

        public DbSet<OgolneStatus> OgolneStatusVocabulary { get; set; }

        public DbSet<OgolneWnioski> OgolneWnioskiVocabulary { get; set; }

        public DbSet<OrganizacjaLokalizacje> OrganizacjaLokalizacjeVocabulary { get; set; }

        public DbSet<ZatrudnieniDzialy> ZatrudnieniDzialy { get; set; }

        public DbSet<ZatrudnieniZrodlo> ZatrudnieniZrodlo { get; set; }

        public DbSet<Kierownicy> Kierownicy { get; set; }
    }
}