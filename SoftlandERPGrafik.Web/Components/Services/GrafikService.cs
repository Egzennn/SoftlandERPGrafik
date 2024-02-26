namespace SoftlandERPGrafik.Web.Components.Services
{
    using Microsoft.EntityFrameworkCore;
    using SoftlandERGrafik.Data.Entities.Forms;
    using SoftlandERPGrafik.Core.Repositories.Interfaces;
    using SoftlandERPGrafik.Data.DB;
    using SoftlandERPGrafik.Data.Entities.Forms.Data;
    using SoftlandERPGrafik.Data.Entities.Views;

    public class GrafikService : BaseService
    {
        private readonly MainContext mainContext;
        private readonly UserDetailsService userDetailsService;
        private readonly IRepository<GrafikForm> grafikRepository;
        private readonly IRepository<OrganizacjaLokalizacje> lokalizacjeRepository;
        private readonly IRepository<ZatrudnieniDzialy> dzialyRepository;
        private readonly IRepository<ZatrudnieniZrodlo> zrodloRepository;

        public GrafikService(IADRepository adRepository, ILogger<BaseService> logger,MainContext mainContext, UserDetailsService userDetailsService, IRepository<GrafikForm> grafikRepository, IRepository<OrganizacjaLokalizacje> lokalizacjeRepository, IRepository<ZatrudnieniDzialy> dzialyRepository, IRepository<ZatrudnieniZrodlo> zrodloRepository)
            : base(adRepository, logger)
        {
            this.mainContext = mainContext;
            this.grafikRepository = grafikRepository;
            this.lokalizacjeRepository = lokalizacjeRepository;
            this.dzialyRepository = dzialyRepository;
            this.zrodloRepository = zrodloRepository;
            this.userDetailsService = userDetailsService;
        }

        public async Task<List<OsobaData>> GetEmployeesAsync()
        {
            var employees = await this.zrodloRepository.GetAllAsync();

            return employees.Select(zatrudniony => new OsobaData(zatrudniony)).OrderBy(dzial => dzial.DZL_Kod).ThenBy(osoba => osoba.Imie_Nazwisko).ToList();
        }

        public async Task<IEnumerable<ZatrudnieniDzialy>> GetDzialyAsync() => await this.dzialyRepository.GetAllAsync();

        public async Task<IEnumerable<OrganizacjaLokalizacje>> GetLocalizationAsync() => await this.lokalizacjeRepository.GetAllAsync();

        public async Task<IEnumerable<GrafikForm>> Get()
        {
            return await this.grafikRepository.GetAllAsync();
        }

        public async Task Insert(GrafikForm appointment)
        {
            var app = new GrafikForm();
            app.StartTime = appointment.StartTime;
            app.EndTime = appointment.EndTime;
            app.IsAllDay = appointment.IsAllDay;
            app.LocationId = appointment.LocationId;
            app.Description = appointment.Description;
            app.RecurrenceRule = appointment.RecurrenceRule;
            app.RecurrenceID = appointment.RecurrenceID;
            app.RecurrenceException = appointment.RecurrenceException;
            await mainContext.GrafikForms.AddAsync(app).ConfigureAwait(true);
            await mainContext.SaveChangesAsync();

        }

        public async Task Update(GrafikForm appointment)
        {
            var app = await mainContext.GrafikForms.FirstAsync(c => c.Id == appointment.Id);

            if (app != null)
            {
                app.StartTime = appointment.StartTime;
                app.EndTime = appointment.EndTime;
                app.IsAllDay = appointment.IsAllDay;
                app.LocationId = appointment.LocationId;
                app.Description = appointment.Description;
                app.RecurrenceRule = appointment.RecurrenceRule;
                app.RecurrenceID = appointment.RecurrenceID;
                app.RecurrenceException = appointment.RecurrenceException;

                mainContext.GrafikForms?.Update(app);
                await mainContext.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid appointmentId)
        {
            var app = await mainContext.GrafikForms.FirstAsync(c => c.Id == appointmentId);

            if (app != null)
            {
                mainContext.GrafikForms?.Remove(app);
                await mainContext.SaveChangesAsync();
            }
        }

    }
}
