using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;
using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.DB;
using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Data.Entities.Forms.Data;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;

namespace SoftlandERPGrafik.Web.Components.Services
{
    public class ScheduleService : BaseService
    {
        private readonly IRepository<ScheduleForm> repository;
        private readonly IRepository<ScheduleHistoryForm> historyRepository;
        private readonly IRepository<Holidays> holidaysRepository;
        private readonly IRepository<OgolneWnioski> wnioskiRepository;
        private readonly IRepository<Kierownicy> kierownicyRepository;

        public ScheduleService(IRepository<ScheduleForm> repository, IRepository<ScheduleHistoryForm> historyRepository, IRepository<Holidays> holidaysRepository, IRepository<OgolneWnioski> wnioskiRepository, IRepository<Kierownicy> kierownicyRepository, MainContext mainContext, ScheduleContext scheduleContext, IADRepository adRepository, ILogger<BaseService> logger, ISnackbar snackbarNotification, UserDetailsService userDetailsService, IRepository<OrganizacjaLokalizacje> lokalizacjeRepository, IRepository<ZatrudnieniDzialy> dzialyRepository, IRepository<ZatrudnieniZrodlo> zrodloRepository, IRepository<OgolneStan> stanRepository, IRepository<OgolneStatus> statusRepository)
            : base(mainContext, scheduleContext, adRepository, logger, snackbarNotification, userDetailsService, lokalizacjeRepository, dzialyRepository, zrodloRepository, stanRepository, statusRepository)
        {
            this.repository = repository;
            this.historyRepository = historyRepository;
            this.holidaysRepository = holidaysRepository;
            this.wnioskiRepository = wnioskiRepository;
            this.kierownicyRepository = kierownicyRepository;
        }

        public async Task<List<OsobaData>> GetEmployeesAsync()
        {
            try
            {
                var employees = await this.zrodloRepository.GetAllAsync();
                return employees.Select(zatrudniony => new OsobaData(zatrudniony)).OrderBy(dzial => dzial.DZL_Kod).ThenBy(osoba => osoba.Imie_Nazwisko).ToList();
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task<IEnumerable<ZatrudnieniDzialy>> GetDepartamentAsync(string acronym)
        {
            try
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
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task<IEnumerable<OrganizacjaLokalizacje>> GetLocalizationAsync()
        {
            try
            {
                this.snackbarNotification.Add("", Severity.Success);
                return await this.lokalizacjeRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task<IEnumerable<Holidays>> GetHolidaysAsync()
        {
            try
            {
                return await this.holidaysRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task<IEnumerable<ScheduleForm>> Get()
        {
            try
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
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task Insert(ScheduleForm appointment)
        {
            try
            {
                var app = new ScheduleForm();
                var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();
                var kierownicy = await this.kierownicyRepository.GetAllAsync();
                var kierownicyAkronim = kierownicy?.Select(x => x.PRI_Opis).ToList();

                app.Type = appointment.Type;
                app.StartTime = appointment.StartTime;
                DateTime endTime = appointment.EndTime;
                if (appointment.RecurrenceRule != null)
                {
                    app.EndTime = appointment.StartTime.Date + endTime.TimeOfDay;
                }
                else
                {
                    app.EndTime = appointment.EndTime;
                }

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
                if (kierownicyAkronim.Contains(userDetails.SamAccountName) || userDetails.SamAccountName == "ASE" || userDetails.SamAccountName == "BWL" || userDetails.SamAccountName == "LSZ")
                {
                    app.Stan = appointment.Stan;
                    if (appointment.Type == "Wniosek")
                    {
                        app.Status = appointment.Status;
                    }
                }
                else
                {
                    app.Stan = "Plan";
                    if (appointment.Type == "Wniosek")
                    {
                        app.Status = "Plan";
                    }
                }

                app.CreatedBy = userDetails?.SamAccountName;

                this.snackbarNotification.Add("Dodano rekord", Severity.Success);
                await this.repository.InsertAsync(app);
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
            }
        }

        public async Task Update(ScheduleForm appointment)
        {
            var app = await this.repository.GetByIdAsync(appointment.Id);
            var userDetails = await this.userDetailsService.GetUserAllDetailsAsync();
            var kierownicy = await this.kierownicyRepository.GetAllAsync();
            var kierownicyAkronim = kierownicy?.Select(x => x.PRI_Opis).ToList();

            try
            {
                if (app != null)
                {
                    var historyRecords = new List<ScheduleHistoryForm>();

                    var propertiesToCheck = new Dictionary<string, Func<ScheduleForm, object>>
                    {
                        { nameof(ScheduleForm.Type), x => x.Type },
                        { nameof(ScheduleForm.StartTime), x => x.StartTime },
                        { nameof(ScheduleForm.EndTime), x => x.EndTime },
                        { nameof(ScheduleForm.Description), x => x.Description },
                        { nameof(ScheduleForm.LocationId), x => x.LocationId },
                        { nameof(ScheduleForm.RequestId), x => x.RequestId },
                        { nameof(ScheduleForm.IDS), x => x.IDS },
                        { nameof(ScheduleForm.IDD), x => x.IDD },
                        { nameof(ScheduleForm.Stan), x => x.Stan },
                        { nameof(ScheduleForm.Status), x => x.Status },
                    };

                    Dictionary<string, string> propertyNameDictionary = new Dictionary<string, string>
                    {
                        { "Type", "Typ" },
                        { "StartTime", "DataPoczatkowa" },
                        { "EndTime", "DataKoncowa" },
                        { "Description", "Notatka" },
                        { "LocationId", "Lokalizacja" },
                        { "RequestId", "Wniosek" },
                    };

                    foreach (var property in propertiesToCheck)
                    {
                        var propertyName = property.Key;
                        var currentValue = property.Value(appointment);
                        var previousValue = app.GetType().GetProperty(propertyName)?.GetValue(app);

                        var translatedPropertyName = propertyNameDictionary.ContainsKey(propertyName) ? propertyNameDictionary[propertyName] : propertyName;

                        if (!Equals(currentValue, previousValue))
                        {
                            historyRecords.Add(new ScheduleHistoryForm
                            {
                                scheduleId = app.Id,
                                Column = translatedPropertyName, // Use translated property name
                                Before = previousValue?.ToString(),
                                After = currentValue?.ToString(),
                                CreatedBy = userDetails?.SamAccountName,
                            });

                            app.GetType().GetProperty(propertyName)?.SetValue(app, currentValue);
                        }
                    }

                    foreach (var historyRecord in historyRecords)
                    {
                        await this.historyRepository.InsertAsync(historyRecord);
                    }

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
                    if (kierownicyAkronim.Contains(userDetails.SamAccountName) || userDetails.SamAccountName == "ASE" || userDetails.SamAccountName == "BWL" || userDetails.SamAccountName == "LSZ")
                    {
                        app.Stan = appointment.Stan;
                        if (appointment.Type == "Wniosek")
                        {
                            app.Status = appointment.Status;
                        }
                    }
                    else
                    {
                        app.Stan = "Plan";
                        if (appointment.Type == "Wniosek")
                        {
                            app.Status = "Plan";
                        }
                    }

                    this.snackbarNotification.Add("Zmodyfikowano rekord", Severity.Info);
                    await this.repository.UpdateAsync(app);
                }
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
            }
        }

        public async Task Delete(Guid appointmentId)
        {
            try
            {
                this.snackbarNotification.Add("Usunięto rekord", Severity.Warning);
                await this.repository.DeleteAsync(appointmentId);
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
            }
        }

        public async Task<IEnumerable<OgolneWnioski>> GetWnioskiAsync()
        {
            try
            {
                var wnioski = await this.wnioskiRepository.GetAllAsync();

                var wnioskiOrder = wnioski?.OrderBy(s => s?.Wartosc);

                return wnioskiOrder;
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return null;
            }
        }

        public async Task<int> CountWeekdaysAmount(DateTime startTime, DateTime endTime)
        {
            try
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
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
                return 0;
            }
        }

        public IEnumerable<ScheduleHistoryForm> GetEventHistory(Guid id)
        {
            try
            {
                var history = this.scheduleContext.ScheduleHistoryForms
                    .Where(x => x.scheduleId == id && x.Column != "Stan" && x.Column != "Status")
                    .ToList();

                return history;
            }
            catch (Exception ex)
            {
                this.snackbarNotification.Add(ex.Message, Severity.Error);
                Log.Fatal(ex, "Fatal error");
            }

            return Enumerable.Empty<ScheduleHistoryForm>();
        }
    }
}
