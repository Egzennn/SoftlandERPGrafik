using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.DB;
using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Data.Entities.Forms.Data;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;
using System.Collections.Generic;
using System.Linq;

namespace SoftlandERPGrafik.Web.Components.Services
{
    public class ScheduleService : BaseService
    {
        private readonly IRepository<ScheduleForm> repository;
        private readonly IRepository<Holidays> holidaysRepository;
        private readonly IRepository<OgolneWnioski> wnioskiRepository;

        public ScheduleService(IRepository<ScheduleForm> repository, IRepository<Holidays> holidaysRepository, IRepository<OgolneWnioski> wnioskiRepository, MainContext mainContext, ScheduleContext scheduleContext, IADRepository adRepository, ILogger<BaseService> logger, UserDetailsService userDetailsService, IRepository<OrganizacjaLokalizacje> lokalizacjeRepository, IRepository<ZatrudnieniDzialy> dzialyRepository, IRepository<ZatrudnieniZrodlo> zrodloRepository, IRepository<OgolneStan> stanRepository, IRepository<OgolneStatus> statusRepository)
            : base(mainContext, scheduleContext, adRepository, logger, userDetailsService, lokalizacjeRepository, dzialyRepository, zrodloRepository, stanRepository, statusRepository)
        {
            this.repository = repository;
            this.holidaysRepository = holidaysRepository;
            this.wnioskiRepository = wnioskiRepository;
        }

        public async Task<List<OsobaData>> GetEmployeesAsync()
        {
            var employees = await this.zrodloRepository.GetAllAsync();

            return employees.Select(zatrudniony => new OsobaData(zatrudniony)).OrderBy(dzial => dzial.DZL_Kod).ThenBy(osoba => osoba.Imie_Nazwisko).ToList();
        }

        public async Task<IEnumerable<ZatrudnieniDzialy>> GetDepartamentAsync(string acronym)
        {
            var employees = await this.GetEmployeesAsync();
            var currentiserdepartament = employees.Where(x => x.PRI_Opis == acronym).Select(x => x.DZL_DzlId);
            var departament = await this.dzialyRepository.GetAllAsync();

            foreach (var dzlId in currentiserdepartament)
            {
                var matchingDepartment = departament?.FirstOrDefault(x => x.DZL_DzlId == dzlId);
                if (matchingDepartment != null)
                {
                    matchingDepartment.IsExpand = true;
                }
            }

            return departament;
        }

        public async Task<IEnumerable<OrganizacjaLokalizacje>> GetLocalizationAsync() => await this.lokalizacjeRepository.GetAllAsync();

        public async Task<IEnumerable<Holidays>> GetHolidaysAsync() => await this.holidaysRepository.GetAllAsync();

        public async Task<IEnumerable<ScheduleForm>> Get()
        {
            var scheduleForms = await this.repository.GetAllAsync();

            //var scheduleForms = await this.scheduleContext.ScheduleForms
            //.ToListAsync().ConfigureAwait(true);

            List<ScheduleForm> eventData = new List<ScheduleForm>();

            foreach (var scheduleForm in scheduleForms)
            {
                eventData.Add(new ScheduleForm
                {
                    Id = scheduleForm.Id,
                    Type = scheduleForm.Type,
                    StartTime = scheduleForm.StartTime,
                    EndTime = scheduleForm.EndTime,
                    LocationId = scheduleForm.LocationId,
                    RequestId = scheduleForm.RequestId,
                    Description = scheduleForm.Description,
                    IDD = scheduleForm.IDD,
                    IDS = scheduleForm.IDS,
                    DaysAmount = scheduleForm.DaysAmount,
                    IsAllDay = scheduleForm.IsAllDay,
                    PRI_PraId = scheduleForm.PRI_PraId,
                    DZL_DzlId = scheduleForm.DZL_DzlId,
                    RecurrenceRule = scheduleForm.RecurrenceRule,
                    RecurrenceID = scheduleForm.RecurrenceID,
                    RecurrenceException = scheduleForm.RecurrenceException,
                    Stan = scheduleForm.Stan,
                    Status = scheduleForm.Status,
                    CreatedBy = scheduleForm.CreatedBy,
                    Color = scheduleForm.Color,
                    Style = scheduleForm.Style,
                });
            }

            return eventData;
        }

        public async Task Insert(ScheduleForm appointment)
        {
            var app = new ScheduleForm();
            var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();

            app.Type = appointment.Type;
            app.StartTime = appointment.StartTime;
            app.EndTime = appointment.EndTime;
            app.LocationId = appointment.LocationId;
            app.RequestId = appointment.RequestId;
            app.Description = appointment.Description;
            app.IDS = appointment.IDS;
            app.IDD = appointment.IDD;
            app.IsAllDay = appointment.IsAllDay;
            if (appointment.Type == "Wniosek")
            {
                app.DaysAmount = await this.CountWeekdaysAmount(appointment.StartTime, appointment.EndTime);
            }

            app.DZL_DzlId = appointment.DZL_DzlId;
            app.PRI_PraId = appointment.PRI_PraId;
            app.RecurrenceRule = appointment.RecurrenceRule;
            app.RecurrenceID = appointment.RecurrenceID;
            app.RecurrenceException = appointment.RecurrenceException;
            app.Stan = "Plan";
            app.Status = "Plan";
            app.CreatedBy = userDetails?.SamAccountName;

            await this.repository.InsertAsync(app);
        }

        public async Task Update(ScheduleForm appointment)
        {
            var app = await this.repository.GetByIdAsync(appointment.Id);
            var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();

            if (app != null)
            {
                app.Type = appointment.Type;
                app.StartTime = appointment.StartTime;
                app.EndTime = appointment.EndTime;
                app.LocationId = appointment.LocationId;
                app.RequestId = appointment.RequestId;
                app.Description = string.IsNullOrWhiteSpace(appointment.Description) ? null : appointment.Description;
                app.IDS = appointment.IDS;
                app.IDD = appointment.IDD;
                if (appointment.Type == "Wniosek")
                {
                    app.DaysAmount = await this.CountWeekdaysAmount(appointment.StartTime, appointment.EndTime);
                }

                app.PRI_PraId = appointment.PRI_PraId;
                app.DZL_DzlId = appointment.DZL_DzlId;
                app.IsAllDay = appointment.IsAllDay;
                app.RecurrenceRule = appointment.RecurrenceRule;
                app.RecurrenceID = appointment.RecurrenceID;
                app.RecurrenceException = appointment.RecurrenceException;
                app.Updated = DateTime.Now;
                app.UpdatedBy = userDetails?.SamAccountName;
                app.Stan = appointment.Stan;
                app.Status = appointment.Status;

                await this.repository.UpdateAsync(app);
            }
        }

        public async Task Delete(Guid appointmentId)
        {
            await this.repository.DeleteAsync(appointmentId);
        }

        public async Task<IEnumerable<OgolneWnioski>> GetWnioskiAsync()
        {
            var wnioski = await this.wnioskiRepository.GetAllAsync();

            var wnioskiOrder = wnioski.OrderBy(s => s?.Wartosc);

            return wnioskiOrder;
        }

        public async Task<int> CountWeekdaysAmount(DateTime startTime, DateTime endTime)
        {
            var daysBetween = Enumerable.Range(0, (int)(endTime - startTime).TotalDays).Select(offset => startTime.AddDays(offset));
            var holidays = await this.GetHolidaysAsync();
            List<DateTime> holidayDateList = holidays.Select(x => x.Data).ToList();

            int weekdaysAmount = 0;

            foreach (var day in daysBetween)
            {
                if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !holidayDateList.Contains(day.Date))
                {
                    weekdaysAmount++;
                }
            }

            return weekdaysAmount;
        }
    }
}
