namespace SoftlandERPGrafik.Web.Components.Services
{
    using SoftlandERPGrafik.Core.Repositories.Interfaces;
    using SoftlandERPGrafik.Data.DB;
    using SoftlandERPGrafik.Data.Entities.Forms;
    using SoftlandERPGrafik.Data.Entities.Views;
    using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;
    using Syncfusion.Blazor.Inputs;

    public class WnioskiService : BaseService
    {
        private readonly IRepository<WnioskiForm> grafikRepository;
        private readonly IRepository<OgolneWnioski> wnioskiRepository;

        public WnioskiService(IRepository<WnioskiForm> grafikRepository, IRepository<OgolneWnioski> wnioskiRepository, MainContext mainContext, IADRepository adRepository, ILogger<BaseService> logger, UserDetailsService userDetailsService, IRepository<OrganizacjaLokalizacje> lokalizacjeRepository, IRepository<ZatrudnieniDzialy> dzialyRepository, IRepository<ZatrudnieniZrodlo> zrodloRepository, IRepository<OgolneStan> stanRepository, IRepository<OgolneStatus> statusRepository)
            : base(mainContext, adRepository, logger, userDetailsService, lokalizacjeRepository, dzialyRepository, zrodloRepository, stanRepository, statusRepository)
        {
            this.grafikRepository = grafikRepository;
            this.wnioskiRepository = wnioskiRepository;
        }

        public async Task<IEnumerable<WnioskiForm>> Get(DateTime startDate, DateTime endDate)
        {
            IEnumerable<WnioskiForm> grafikForms = this.mainContext.WnioskiForms.Where(e => e.StartTime >= startDate && e.EndTime <= endDate);
            //IEnumerable<WnioskiForm> grafikForms = this.mainContext.GrafikForms.Where(e => e.Id == new Guid("11b0f9d2-cef5-430f-a4d7-a26ce5e6ec9b"));

            DateTime selectedDate = DateTime.UtcNow.ToLocalTime();

            List<WnioskiForm> eventData = new List<WnioskiForm>();

            foreach (var grafikForm in grafikForms)
            {
                eventData.Add(new WnioskiForm
                {
                    Id = grafikForm.Id,
                    StartTime = grafikForm.StartTime,
                    EndTime = grafikForm.EndTime,
                    RequestId = grafikForm.RequestId,
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

        public async Task Insert(WnioskiForm appointment)
        {
            var app = new WnioskiForm();
            var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();

            app.StartTime = appointment.StartTime;
            app.EndTime = appointment.EndTime;
            app.PRI_PraId = appointment.PRI_PraId;
            app.DZL_DzlId = appointment.DZL_DzlId;
            app.IsAllDay = appointment.IsAllDay;
            app.RequestId = appointment.RequestId;
            app.Description = appointment.Description;
            app.IDD = appointment.IDD;
            app.IDS = appointment.IDS;
            app.Description = appointment.Description;
            app.RecurrenceRule = appointment.RecurrenceRule;
            app.RecurrenceID = appointment.RecurrenceID;
            app.RecurrenceException = appointment.RecurrenceException;
            app.CreatedBy = userDetails?.SamAccountName;
            app.Stan = "Plan";

            await this.grafikRepository.InsertAsync(app);
        }

        public async Task Update(WnioskiForm appointment)
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
                app.RequestId = appointment.RequestId;
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

        public async Task<IEnumerable<OgolneWnioski>> GetWnioskiAsync()
        {
            var wnioski = await this.wnioskiRepository.GetAllAsync();

            var wnioskiOrder = wnioski.OrderBy(s => s?.Wartosc);

            return wnioskiOrder;
        }
    }
}
