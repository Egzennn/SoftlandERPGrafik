namespace SoftlandERPGrafik.Web.Components.Services
{
    using Microsoft.EntityFrameworkCore;
    using SoftlandERPGrafik.Data.Entities.Forms;
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

        public async Task<IEnumerable<GrafikForm>> Get(DateTime startDate, DateTime endDate)
        {
            IEnumerable<GrafikForm> grafikForms = this.mainContext.GrafikForms.Where(e => e.StartTime >= startDate && e.EndTime <= endDate);
            //IEnumerable<GrafikForm> grafikForms = this.mainContext.GrafikForms.Where(e => e.Id == new Guid("11b0f9d2-cef5-430f-a4d7-a26ce5e6ec9b"));

            DateTime selectedDate = DateTime.UtcNow.ToLocalTime();

            List<GrafikForm> eventData = new List<GrafikForm>();

            foreach (var grafikForm in grafikForms)
            {
                eventData.Add(new GrafikForm
                {
                    Id = grafikForm.Id,
                    StartTime = grafikForm.StartTime,
                    EndTime = grafikForm.EndTime,
                    LocationId = grafikForm.LocationId,
                    Description = grafikForm.Description,
                    IsAllDay = grafikForm.IsAllDay,
                    PRI_PraId = grafikForm.PRI_PraId,
                    DZL_DzlId = grafikForm.DZL_DzlId,
                    RecurrenceID = grafikForm.RecurrenceID,
                    RecurrenceRule = grafikForm.RecurrenceRule,
                    RecurrenceException = grafikForm.RecurrenceException,
                    //IsReadonly = true,
                    Stan = grafikForm.Stan,
                    Status = grafikForm.Status,
                    CreatedBy = grafikForm.CreatedBy,
                    Color = grafikForm.Color,
                    Style = grafikForm.Style,
                });
            }

            return eventData;
        }

        public async Task Insert(GrafikForm appointment)
        {
            var app = new GrafikForm();
            var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();

            app.StartTime = appointment.StartTime;
            app.EndTime = appointment.EndTime;
            app.PRI_PraId = appointment.PRI_PraId;
            app.DZL_DzlId = appointment.DZL_DzlId;
            app.IsAllDay = appointment.IsAllDay;
            app.LocationId = appointment.LocationId;
            app.Description = appointment.Description;
            app.RecurrenceRule = appointment.RecurrenceRule;
            app.RecurrenceID = appointment.RecurrenceID;
            app.RecurrenceException = appointment.RecurrenceException;
            app.CreatedBy = userDetails?.SamAccountName;
            app.Stan = "Plan";

            await this.grafikRepository.InsertAsync(app);
        }

        public async Task Update(GrafikForm appointment)
        {
            var app = await this.grafikRepository.GetByIdAsync(appointment.Id);
            var userDetails = await userDetailsService.GetUserAllDetailsAsync();

            if (app != null)
            {
                app.StartTime = appointment.StartTime;
                app.EndTime = appointment.EndTime;
                app.PRI_PraId = appointment.PRI_PraId;
                app.DZL_DzlId = appointment.DZL_DzlId;
                app.IsAllDay = appointment.IsAllDay;
                app.LocationId = appointment.LocationId;
                app.Description = string.IsNullOrWhiteSpace(appointment.Description) ? null : appointment.Description;
                app.RecurrenceRule = appointment.RecurrenceRule;
                app.RecurrenceID = appointment.RecurrenceID;
                app.RecurrenceException = appointment.RecurrenceException;
                app.Updated = DateTime.Now;
                app.UpdatedBy = userDetails?.SamAccountName;
                app.Stan = appointment.Stan;

                await this.grafikRepository.UpdateAsync(app);
            }
        }

        public async Task Delete(Guid appointmentId)
        {
            await this.grafikRepository.DeleteAsync(appointmentId);
        }
    }
}
