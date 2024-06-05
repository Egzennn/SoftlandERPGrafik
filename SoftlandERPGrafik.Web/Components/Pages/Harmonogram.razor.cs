namespace SoftlandERPGrafik.Web.Components.Pages
{
    using System.DirectoryServices.AccountManagement;
    using System.Globalization;
    using System.Timers;
    using Microsoft.AspNetCore.Components.Web;
    using MudBlazor;
    using SoftlandERPGrafik.Data.Entities.Forms;
    using SoftlandERPGrafik.Data.Entities.Forms.Data;
    using SoftlandERPGrafik.Data.Entities.Staff.AD;
    using SoftlandERPGrafik.Data.Entities.Views;
    using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;
    using Syncfusion.Blazor;
    using Syncfusion.Blazor.Data;
    using Syncfusion.Blazor.DropDowns;
    using Syncfusion.Blazor.Inputs;
    using Syncfusion.Blazor.Navigations;
    using Syncfusion.Blazor.Schedule;
    using Timer = System.Timers.Timer;

    public partial class Harmonogram
    {
        private IEnumerable<ADUser>? aduser;
        private UserPrincipal? userDetails;
        private bool IsDzialyExpanded = false;
        private Query DepartmanetQuery { get; set; } = new Query();
        private Query ResourceQuery { get; set; } = new Query();
        private Query LocalizationQuery { get; set; } = new Query();
        SfTextBox SubjectRef;
        SfMultiSelect<int[], ZatrudnieniDzialy>? DepartamentRef;
        SfMultiSelect<int[], OsobaData>? ResourceRef;
        SfMultiSelect<int[], OsobaData>? ResourceRef2;
        SfMultiSelect<int[], OrganizacjaLokalizacje>? LocationRef;
        SfMultiSelect<Guid[], OgolneWnioski>? RequestRef;
        SfSchedule<ScheduleForm>? ScheduleRef;
        bool isCreated;
        private Timezone TimezoneData { get; set; } = new Timezone();
        private IEnumerable<OsobaData>? Osoby;
        private IEnumerable<OsobaData>? OsobyForDialog;
        private IEnumerable<ZatrudnieniDzialy>? Dzialy;
        private IEnumerable<Kierownicy>? Kierownik;
        private IEnumerable<OrganizacjaLokalizacje>? LocalizationData;
        private static IEnumerable<OgolneWnioski>? WnioskiData;
        private IEnumerable<OgolneWnioski>? wnioskiUrlop;
        public IEnumerable<Holidays>? Holiday;
        private List<string?> ogolneStatusy;
        private List<ScheduleForm>? gridDataSource;
        private IEnumerable<ScheduleHistoryForm>? eventHistorygridDataSource;
        IEnumerable<ScheduleForm> DataSource;
        private bool ShowSchedule { get; set; } = true;
        private bool ShowEventHistory { get; set; } = true;
        private string SearchValue { get; set; }
        public ScheduleForm EventData { get; set; }
        public CellClickEventArgs CellData { get; set; }
        private bool isCell { get; set; }
        private bool isEvent { get; set; }
        private bool isRecurrence { get; set; }
        private int SlotCount { get; set; } = 2;
        private int SlotInterval { get; set; } = 60;
        private int FirstDayOfWeek { get; set; } = 1;
        private bool TooltipEnable { get; set; } = true;
        private bool isRowAutoHeight { get; set; } = true;
        private bool EnableTimeScale { get; set; } = false;
        private bool isQuickInfoCreated { get; set; } = false;
        private View CurrentView { get; set; } = View.TimelineMonth;
        private string TimeFormat { get; set; } = "HH:mm";
        private string DateFormat { get; set; } = "yyyy-MM-dd";
        private bool IsSettingsVisible { get; set; } = false;
        public string[] GroupData = new string[] { "Dzialy", "Osoby" };
        public string[] Type = new string[] { "Grafik", "Wniosek" };
        private DateTime SystemTime { get; set; } = DateTime.UtcNow.ToLocalTime();
        private DateTime SelectedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        private bool disableState = false;
        private bool enableStateA = false;
        private bool enableStateB = false;
        private bool showSecondMultiSelect = false;
        private bool showThirdMultiSelect = false;
        private List<string>? signedInGroup;
        private string headerEdit = "Edycja wydarzenia/serii";
        private string headerDeleteSeries = "Usunięcie wydarzenia/serii";
        private string headerDelete = "Usuń wydarzenie";
        private string headerEditStan = "Masowa edycja stanów";
        private string headerExport = "Export";
        private string[] cellCustomClass = { "cell-custom-class" };
        private string[] customClass = { "custom-class" };

        private int[] SelectedPRI_PraId { get; set; }

        private string SelectedType { get; set; }

        private bool VisibilityEdit { get; set; } = false;

        private bool VisibilityDelete { get; set; } = false;

        private bool VisibilityDeleteSeries { get; set; } = false;

        private bool VisibilityChangeStanAll { get; set; } = false;

        private bool VisibilityExport { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            this.aduser = this.ADRepository.GetAllADUsers();
            this.userDetails = await this.UserDetailsService.GetUserAllDetailsAsync();
            this.Osoby = await this.ScheduleService.GetEmployeesAsync();
            this.Dzialy = await this.ScheduleService.GetDepartamentAsync(this.userDetails?.SamAccountName);
            this.TimezoneData = new Timezone().GetSystemTimeZone();
            this.LocalizationData = await this.ScheduleService.GetLocalizationAsync();
            WnioskiData = await this.ScheduleService.GetWnioskiAsync();
            this.Holiday = await this.ScheduleService.GetHolidaysAsync();
            this.Kierownik = await this.Kierownicy.GetAllAsync();
            this.ogolneStatusy = await this.ScheduleService.GetStatusAsync();
            this.signedInGroup = this.ScheduleService.GetSignedInGroups(this.userDetails?.SamAccountName);
            this.DataSource = await this.ScheduleService.Get();
            this.wnioskiUrlop = WnioskiData.Where(x => x.Wartosc.Contains("(Uw"));
            this.OsobyForDialog = await this.GetOsobaByDepartament();
        }

        public async Task ChangeStanAll()
        {
            try
            {
                string type = this.SelectedType;
                List<int> ids = this.SelectedPRI_PraId.ToList();
                List<DateTime> viewDates = this.ScheduleRef.GetCurrentViewDates();
                DateTime startDate = viewDates.Min();
                DateTime endDate = viewDates.Max();

                var eventsChangeStan = this.DataSource
                    .Where(x => ids.Contains(x.PRI_PraId) && x.Type == type && x.StartTime >= startDate && x.EndTime <= endDate && x.Stan == "Plan")
                    .ToList();

                int recordsUpdated = 0;
                foreach (var eventData in eventsChangeStan)
                {
                    eventData.Updated = DateTime.Now;
                    eventData.UpdatedBy = this.userDetails?.SamAccountName;
                    if (this.userDetails?.SamAccountName == "ASE")
                    {
                        eventData.Status = "Akceptacja";
                    }
                    else
                    {
                        eventData.Stan = "Akceptacja";
                    }

                    await this.ScheduleService.Update(eventData);
                    recordsUpdated++;
                }

                if (recordsUpdated == 0)
                {
                    this.Snackbar.Add("Brak rekordów do zmiany", Severity.Error);
                }
                else
                {
                    this.Snackbar.Add($"Zaktualizowano {recordsUpdated} rekordów.", Severity.Success);
                    this.StateHasChanged();
                    await this.ScheduleRef.RefreshEventsAsync();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<List<OsobaData>> GetOsobaByDepartament()
        {
            List<OsobaData> listaOsob = new List<OsobaData>(); // Inicjalizacja listy poza warunkami warunkowymi

            if (this.signedInGroup.Contains("S_ADM_IT") || this.signedInGroup.Contains("S_ADM_KADRY"))
            {
                listaOsob = this.Osoby.ToList();
            }
            else
            {
                int praId = this.Kierownik
                    .Where(x => x.PRI_Opis == this.userDetails?.SamAccountName)
                    .Select(x => x.PRI_PraId)
                    .FirstOrDefault();

                int dzialId = this.Osoby
                    .Where(x => x.PRI_PraId == praId)
                    .Select(x => x.DZL_DzlId)
                    .FirstOrDefault();

                listaOsob = this.Osoby
                    .Where(x => x.DZL_DzlId == dzialId)
                    .ToList();
            }

            return listaOsob;
        }

        public int[] GetDaysAmount(int id)
        {
            try
            {
                int daysCountPlan = this.DataSource
                    .Where(x => x.Type == "Wniosek"
                                && (x.Stan == "Plan" || x.Status == "Plan")
                                && x.PRI_PraId == id
                                && this.wnioskiUrlop.Any(wn => wn.Id == x.RequestId))
                    .Sum(x => x.DaysAmount ?? 0);

                int daysCountAkceptacja = this.DataSource
                    .Where(x => x.Type == "Wniosek" && (x.Stan == "Akceptacja" && x.Status == "Akceptacja") && x.PRI_PraId == id && this.wnioskiUrlop.Any(wn => wn.Id == x.RequestId))
                    .Sum(x => x.DaysAmount ?? 0);

                return new int[] { daysCountPlan, daysCountAkceptacja };
            }
            catch (Exception ex)
            {
                // Tutaj możesz obsłużyć błąd, np. zalogować go lub rzucić dalej
                throw new Exception("Błąd podczas pobierania liczby dni.", ex);
            }
        }

        private bool CheckEventHistory(Guid id)
        {
            try
            {
                var eventHistory = this.ScheduleService.GetEventHistory(id);

                // Check if eventHistory is null or empty
                return eventHistory == null || !eventHistory.Any();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"An error occurred while checking event history: {ex.Message}");
                return false; // Or handle it based on your application's requirements
            }
        }

        public async void OnRenderCell(RenderCellEventArgs args)
        {
            var item = args.Date;
            HolidaysDate holidays = new HolidaysDate();
            bool isHoliday = holidays.dateCollection.Any(d => d.Year == item.Year && d.Month == item.Month && d.Day == item.Day);
            bool isWeekend = args.Date.DayOfWeek == DayOfWeek.Sunday || args.Date.DayOfWeek == DayOfWeek.Saturday;

            if (args.ElementType == ElementType.WorkCells)
            {
                if (isHoliday)
                {
                    args.CssClasses = new List<string>(this.cellCustomClass);
                }
                else
                {
                    args.CssClasses = new List<string>(this.customClass);
                }
            }
            else if (isHoliday)
            {
                args.CssClasses = new List<string>(this.cellCustomClass);
            }
        }

        private async Task EditOccurrence()
        {
            await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.EditOccurrence);
            this.VisibilityEdit = false;
        }

        private async void EditSeries()
        {
            List<ScheduleForm> events = await this.ScheduleRef.GetEventsAsync();
            this.EventData = (ScheduleForm)events.Where(data => data.Id == this.EventData.RecurrenceID).FirstOrDefault();
            await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.EditSeries);
            this.VisibilityEdit = false;
        }

        private async void DeleteEvent()
        {
            await this.ScheduleRef.DeleteEventAsync(this.EventData, !string.IsNullOrEmpty(this.EventData.RecurrenceRule) ? CurrentAction.DeleteOccurrence : CurrentAction.Delete);
            this.VisibilityDelete = false;
            this.StateHasChanged();
            await this.ScheduleRef.RefreshEventsAsync();
        }

        private async void DeleteOccurrence()
        {
            await this.ScheduleRef.DeleteEventAsync(this.EventData, CurrentAction.DeleteOccurrence);
            this.VisibilityDeleteSeries = false;
            this.StateHasChanged();
            await this.ScheduleRef.RefreshEventsAsync();
        }

        private async void DeleteSeries()
        {
            await this.ScheduleRef.DeleteEventAsync(this.EventData, CurrentAction.DeleteSeries);
            this.VisibilityDeleteSeries = false;
            this.StateHasChanged();
            await this.ScheduleRef.RefreshEventsAsync();
        }

        private void OnTypeChange(object value)
        {
            string[] selectedType = value as string[];

            if (selectedType != null && selectedType.Contains("Grafik") && !selectedType.Contains("Wniosek"))
            {
                this.showSecondMultiSelect = true;
                this.showThirdMultiSelect = false;
            }
            else if (selectedType != null && selectedType.Contains("Wniosek") && !selectedType.Contains("Grafik"))
            {
                this.showSecondMultiSelect = false;
                this.showThirdMultiSelect = true;
            }
            else if (selectedType != null && selectedType.Contains("Wniosek") && selectedType.Contains("Grafik"))
            {
                this.showSecondMultiSelect = true;
                this.showThirdMultiSelect = true;
            }
            else
            {
                this.showSecondMultiSelect = false;
                this.showThirdMultiSelect = false;
            }

            this.StateHasChanged();
        }

        private async Task OnChangeUpload(UploadChangeEventArgs args)
        {
            try
            {
                var sciezka = @"\\10.10.0.2\Poland\Wnioski\";

                foreach (var file in args.Files)
                {
                    var nazwaBezRozszerzenia = Path.GetFileNameWithoutExtension(file.FileInfo.Name);
                    var rozszerzenie = Path.GetExtension(file.FileInfo.Name);
                    var numer = 1;
                    var nazwa = this.userDetails?.SamAccountName + "_" + nazwaBezRozszerzenia;

                    if (File.Exists(Path.Combine(sciezka, nazwa + rozszerzenie)))
                    {
                        while (File.Exists(Path.Combine(sciezka, nazwa + "_" + numer + rozszerzenie)))
                        {
                            numer++;
                        }

                        nazwa = nazwa + "_" + numer;
                    }

                    var path = Path.Combine(sciezka, nazwa + rozszerzenie);

                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        await file.File.OpenReadStream(long.MaxValue).CopyToAsync(filestream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnRemoveUpload(RemovingEventArgs args)
        {
            var sciezka = @"\\10.10.0.2\Poland\Wnioski\";

            foreach (var removeFile in args.FilesData)
            {
                var nazwaBezRozszerzenia = Path.GetFileNameWithoutExtension(removeFile.Name);
                var rozszerzenie = Path.GetExtension(removeFile.Name);
                var nazwa = this.userDetails?.SamAccountName + "_" + nazwaBezRozszerzenia;

                int najwyzszyNumer = 0;

                for (int numer = 1; numer <= 999; numer++)
                {
                    var potencjalnaSciezka = Path.Combine(sciezka, $"{nazwa}_{numer}{rozszerzenie}");

                    if (File.Exists(potencjalnaSciezka))
                    {
                        najwyzszyNumer = Math.Max(najwyzszyNumer, numer);
                    }
                }

                var sciezkaPodstawowa = Path.Combine(sciezka, $"{nazwa}{rozszerzenie}");
                var sciezkaZNumerem = Path.Combine(sciezka, $"{nazwa}_{najwyzszyNumer}{rozszerzenie}");

                if (File.Exists(sciezkaPodstawowa) || (najwyzszyNumer > 0 && File.Exists(sciezkaZNumerem)))
                {
                    if (najwyzszyNumer > 0 && File.Exists(sciezkaZNumerem))
                    {
                        File.Delete(sciezkaZNumerem);
                    }
                    else if (File.Exists(sciezkaPodstawowa))
                    {
                        File.Delete(sciezkaPodstawowa);
                    }
                }
            }
        }

        private void SetDisableState()
        {
            var priPraId = this.EventData?.PRI_PraId;
            var isAuthorized = this.IsCurrentUserAuthorized(priPraId);
            var dzlDzlId = this.EventData?.DZL_DzlId;
            var isAuthorizedManager = this.DepartmentManagerCRUD();

            this.disableState = !(priPraId != 0 && (isAuthorized || (isAuthorizedManager.HasValue && isAuthorizedManager.Value == dzlDzlId)));
        }

        private string? GetADUserInfo(string pRI_Opis)
        {
            if (string.IsNullOrEmpty(pRI_Opis))
            {
                return null;
            }
            else
            {
                var matchingUser = this.aduser?.FirstOrDefault(user => user.Login == pRI_Opis);

                if (matchingUser == null)
                {
                    return "Pracownik";
                }
                else
                {
                    string? jobTitle = matchingUser?.JobTitle;
                    return jobTitle;
                }
            }
        }

        private bool IsCurrentUserAuthorized(int? priPraId)
        {
            var priOpis = this.Osoby.FirstOrDefault(o => o.PRI_PraId == priPraId)?.PRI_Opis;
            return this.userDetails?.SamAccountName == priOpis;
        }

        private int? DepartmentManagerCRUD()
        {
            bool isManager = this.Kierownik.Any(o => o.PRI_Opis == this.userDetails?.SamAccountName);

            if (isManager)
            {
                var priOpis = this.Kierownik.FirstOrDefault(o => o.PRI_Opis == this.userDetails?.SamAccountName)?.CNT_Nazwa;
                int idDzial = this.Dzialy.FirstOrDefault(o => string.Equals(o.DZL_Kod, priOpis, StringComparison.OrdinalIgnoreCase)).DZL_DzlId;

                return idDzial;
            }

            return null;
        }

        private OsobaData? GetOsobaBySamAccountName(string samAccountName)
        {
            if (this.Osoby != null)
            {
                return this.Osoby.FirstOrDefault(osoba => osoba.PRI_Opis == samAccountName);
            }

            return null;
        }

        // Style
        public async Task OnEventRendered(EventRenderedArgs<ScheduleForm> args)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            DateTime endTime = args.Data.EndTime;
            var eventHistory = this.CheckEventHistory(args.Data.Id);
            bool delegacja = args.Data.RequestId == Guid.Parse("2493C407-54C9-4B2F-82C6-166E1865A165");

            string backgroundColor = args.Data.Color;
            string borderColor = "border-color: rgba(42, 65, 111, 0.2)";
            string backgroundImage = args.Data.Style;

            if (!eventHistory)
            {
                attributes.Add("style", $"background:{backgroundColor}; border-color:{borderColor}; background-image:{backgroundImage}; border: #0d6efd; border-left-style: solid;");
            }
            else
            {
                attributes.Add("style", $"background:{backgroundColor}; border-color:{borderColor}; background-image:{backgroundImage}");
            }

            args.Attributes = attributes;
            if (endTime <= this.SystemTime)
            {
                args.Data.IsReadonly = true;
                attributes.Add("class", "e-read-only");
            }

            var item = args.Data.StartTime;
            HolidaysDate holidays = new HolidaysDate();
            bool exist = holidays.dateCollection.Any(d => d.Year == item.Year && d.Month == item.Month && d.Day == item.Day);
            if (exist && !delegacja)
            {
                args.Cancel = true;
            }
        }

        public void OnPopupOpen(PopupOpenEventArgs<ScheduleForm> args)
        {
            var item = args.Data.StartTime;
            HolidaysDate holidays = new HolidaysDate();
            bool exist = holidays.dateCollection.Any(d => d.Year == item.Year && d.Month == item.Month && d.Day == item.Day);
            if (exist)
            {
                args.Cancel = true;
            }
        }

        // Wyszukiwarka
        private T GetDataById<T, TKey>(TKey id, IEnumerable<T> data, Func<T, TKey> idSelector, Func<T> createDefault)
        {
            var item = data.FirstOrDefault(d => idSelector(d).Equals(id));
            return item ?? createDefault();
        }

        private ZatrudnieniDzialy GetDzialDataByDzlId(int id)
        {
            return this.GetDataById(id, Dzialy, d => d.DZL_DzlId, () => new ZatrudnieniDzialy());
        }

        private OsobaData GetOsobaDataByDzlId(int id)
        {
            return this.GetDataById(id, Osoby, d => d.PRI_PraId, () => new OsobaData(new ZatrudnieniZrodlo()));
        }

        private OrganizacjaLokalizacje GetLocationDataByLokId(int? id)
        {
            return this.GetDataById(id, this.LocalizationData, d => d.Lok_LokId, () => new OrganizacjaLokalizacje());
        }

        private OgolneWnioski GetWniosekDataByRequestId(Guid requestId)
        {
            return GetDataById(requestId, WnioskiData, w => w.Id, () => new OgolneWnioski());
        }

        public async Task OnEventSearch()
        {
            if (!string.IsNullOrEmpty(this.SearchValue) && this.ScheduleRef != null)
            {
                Query query = new Query().Search(this.SearchValue, new List<string> { "Description" }, null, true, true);
                List<ScheduleForm> eventCollections = await this.ScheduleRef.GetEventsAsync(null, null, true);
                object data = await new DataManager() { Json = eventCollections }.ExecuteQuery<ScheduleForm>(query);
                List<ScheduleForm>? resultData = data as List<ScheduleForm>;
                switch (resultData?.Count)
                {
                    case > 0:
                        this.ShowSchedule = false;
                        this.gridDataSource = resultData;
                        break;
                    default:
                        this.ShowSchedule = true;
                        this.Snackbar.Add("Brak wyników wyszukiwania", Severity.Error);
                        break;
                }
            }
            else
            {
                this.ShowSchedule = true;
            }
        }

        public async Task GetEventHistoryGrid(Guid id)
        {
            IEnumerable<ScheduleHistoryForm> resultData = this.ScheduleService.GetEventHistory(id);

            if (resultData != null && resultData.Any())
            {
                this.ShowEventHistory = false;
                this.eventHistorygridDataSource = resultData;
            }
            else
            {
                this.ShowEventHistory = true;
                this.Snackbar.Add("Brak wyników wyszukiwania", Severity.Error);
            }
        }

        public void OnPracownikChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<int[]> args)
        {
            int[] ids = args.Value;
            SelectedPRI_PraId = ids != null ? ids : new int[0];
        }

        public void OnTypeChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<string> args)
        {
            string type = args.Value;
            this.SelectedType = type;
        }

        public void OnMultiSelectChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<int[]> args, string field)
        {
            WhereFilter predicate;

            if (args.Value != null && args.Value.Length > 0)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = field,
                    Operator = "equal",
                    value = id,
                });

                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));

                if (field == "LocationId")
                {
                    this.LocalizationQuery = new Query().AddParams("LocationId", args.Value);
                }
                else
                {
                    this.ResourceQuery = new Query().Where(predicate);
                }
            }
            else
            {
                if (field == "LocationId")
                {
                    this.LocalizationQuery = new Query();
                }
                else
                {
                    this.ResourceQuery = new Query();
                }
            }
        }

        public void OnMultiSelectTypeChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<string[]> args, string field)
        {
            WhereFilter predicate;

            if (args.Value != null)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = field,
                    Operator = "equal",
                    value = id,
                });

                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));
                this.LocalizationQuery = new Query().AddParams("Type", args.Value);
            }
            else
            {
                this.LocalizationQuery = new Query();
            }
        }

        public void OnMultiSelectRequestChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<Guid[]> args, string field)
        {
            WhereFilter predicate;

            if (args.Value != null)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = field,
                    Operator = "equal",
                    value = id,
                });

                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));
                this.LocalizationQuery = new Query().AddParams("RequestId", args.Value);
            }
            else
            {
                this.LocalizationQuery = new Query();
            }
        }

        private async void ChangeVisibility()
        {
            this.VisibilityChangeStanAll = true;
        }

        private async void OnNewEventAdd()
        {
            DateTime date = this.ScheduleRef.SelectedDate;
            DateTime start = new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, 0, 0);
            var samaccountname = this.userDetails?.SamAccountName;
            OsobaData? user = this.GetOsobaBySamAccountName(samaccountname);
            ScheduleForm eventData = new ScheduleForm
            {
                Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                Type = "Grafik",
                StartTime = start,
                EndTime = start.AddHours(1),
                IsAllDay = false,
                PRI_PraId = user.PRI_PraId,
                DZL_DzlId = user.DZL_DzlId,
                Stan = "Plan",
            };
            await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
        }

        private async void OnNewRecurringEventAdd()
        {
            DateTime date = this.ScheduleRef.SelectedDate;
            DateTime start = new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, 0, 0);
            var samaccountname = this.userDetails?.SamAccountName;
            OsobaData? user = this.GetOsobaBySamAccountName(samaccountname);
            ScheduleForm eventData = new ScheduleForm
            {
                Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                Type = "Grafik",
                StartTime = start,
                EndTime = start.AddHours(1),
                IsAllDay = false,
                PRI_PraId = user.PRI_PraId,
                DZL_DzlId = user.DZL_DzlId,
                RecurrenceRule = "FREQ=DAILY;INTERVAL=1;",
                Stan = "Plan",
            };
            await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
        }

        private async void OnMoreDetailsClick(MouseEventArgs args, ScheduleForm data, bool isEventData)
        {
            await this.ScheduleRef.CloseQuickInfoPopupAsync();
            if (isEventData == false)
            {
                data.Type = "Grafik";
                data.Stan = "Plan";
                data.Status = "Plan";
                await this.ScheduleRef.OpenEditorAsync(data, CurrentAction.Add);
            }
            else
            {
                ScheduleForm eventData = new ScheduleForm
                {
                    Id = data.Id,
                    Type = data.Type,
                    StartTime = data.StartTime,
                    EndTime = data.EndTime,
                    LocationId = data.LocationId,
                    RequestId = data.RequestId,
                    Description = data.Description,
                    IDD = data.IDD,
                    IDS = data.IDS,
                    DaysAmount = data.DaysAmount,
                    IsAllDay = data.IsAllDay,
                    PRI_PraId = data.PRI_PraId,
                    DZL_DzlId = data.DZL_DzlId,
                    RecurrenceRule = data.RecurrenceRule,
                    RecurrenceID = data.RecurrenceID,
                    RecurrenceException = data.RecurrenceException,
                    Stan = data.Stan,
                    Status = data.Status,
                    CreatedBy = data.CreatedBy,
                    Color = data.Color,
                    Style = data.Style,
                };

                if (data.RecurrenceRule != null)
                {
                    this.VisibilityEdit = true;
                    this.EventData = eventData;
                }
                else
                {
                    await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Save);
                }
            }
        }

        private async Task OnDelete(ScheduleForm data)
        {
            await this.ScheduleRef.CloseQuickInfoPopupAsync();
            this.EventData = data;
            if (data.RecurrenceRule != null)
            {
                this.VisibilityDeleteSeries = true;
            }
            else
            {
                this.VisibilityDelete = true;
            }
        }

        private async Task OnToggleDzialyButton()
        {
            var dzialyIds = Dzialy.Select(d => d.DZL_DzlId).Distinct().ToList();

            foreach (var dzialId in dzialyIds)
            {
                if (this.IsDzialyExpanded)
                {
                    await this.ScheduleRef.CollapseResourceAsync(dzialId, "Dzialy");
                }
                else
                {
                    await this.ScheduleRef.ExpandResourceAsync(dzialId, "Dzialy");
                }
            }

            this.IsDzialyExpanded = !this.IsDzialyExpanded;
        }

        private async Task PopupClose()
        {
            await this.ScheduleRef.CloseQuickInfoPopupAsync();
        }

        private void OnViewButtonClick(View targetView)
        {
            if (targetView == View.TimelineYear)
            {
                this.isRowAutoHeight = false;
            }
            else
            {
                this.isRowAutoHeight = true;
            }

            this.CurrentView = targetView;
        }

        private string GetEventDetails(ScheduleForm data)
        {
            if (data.Type == "Grafik")
            {
                return data.StartTime.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture) + " (" + data.StartTime.ToString(TimeFormat, CultureInfo.CurrentCulture) + " - " + data.EndTime.ToString(TimeFormat, CultureInfo.CurrentCulture) + ")";
            }
            else
            {
                return data.StartTime.ToString(this.DateFormat, CultureInfo.CurrentCulture) + " - " + data.EndTime.AddDays(-1).ToString(this.DateFormat, CultureInfo.CurrentCulture);
            }
        }

        private async Task SetFocus()
        {
            if (this.isQuickInfoCreated)
            {
                await Task.Delay(20);
                await this.SubjectRef.FocusAsync();
            }
        }

        private void OnToolbarCreated()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>

            {
                string key = this.TimezoneData.Key ?? "UTC";
                this.SystemTime = this.TimeConvertor(key);
                this.ScheduleRef?.PreventRender();
                this.InvokeAsync(() => { this.StateHasChanged(); });
            });
            timer.Enabled = true;
        }

        private DateTime TimeConvertor(string timeZoneId)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }

        private async Task OnFileUploadChange(UploadChangeEventArgs args)
        {
            foreach (Syncfusion.Blazor.Inputs.UploadFiles file in args.Files)
            {
                StreamReader reader = new StreamReader(file.File.OpenReadStream(long.MaxValue));
                string fileContent = await reader.ReadToEndAsync();
                await this.ScheduleRef.ImportICalendarAsync(fileContent);
            }
        }

        private async void OnPrintClick()
        {
            await this.ScheduleRef.PrintAsync();
        }

        private async void OnExportClick(Syncfusion.Blazor.SplitButtons.MenuEventArgs args)
        {
            List<ScheduleForm> exportDatas = new List<ScheduleForm>();
            List<ScheduleForm> eventCollection = await this.ScheduleRef.GetEventsAsync();
            List<Syncfusion.Blazor.Schedule.Resource> resourceCollection = this.ScheduleRef.GetResourceCollections();
            List<OsobaData>? resourceData = resourceCollection[0].DataSource as List<OsobaData>;
            List<ExportFieldInfo> exportFields = new List<ExportFieldInfo>();
            exportFields.Add(new ExportFieldInfo { Name = "Dzial", Text = "Dzial" });
            exportFields.Add(new ExportFieldInfo { Name = "Imie_Nazwisko", Text = "Imie i Nazwisko" });
            exportFields.Add(new ExportFieldInfo { Name = "Lokalizacja", Text = "Lokalizacja" });
            exportFields.Add(new ExportFieldInfo { Name = "Wniosek", Text = "Wniosek" });
            exportFields.Add(new ExportFieldInfo { Name = "Description", Text = "Opis" });
            exportFields.Add(new ExportFieldInfo { Name = "StartTime", Text = "Od" });
            exportFields.Add(new ExportFieldInfo { Name = "EndTime", Text = "Do" });

            if (args.Item.Text == "Grafik")
            {
                foreach (var osoba in Osoby)
                {
                    List<ScheduleForm> datas = (from data in eventCollection
                                                join os in Osoby on data.PRI_PraId equals os.PRI_PraId
                                                join dzial in Dzialy on data.DZL_DzlId equals dzial.DZL_DzlId
                                                join lokalizacja in LocalizationData on data.LocationId equals lokalizacja.Lok_LokId
                                                where os.PRI_PraId == osoba.PRI_PraId && data.Type == "Grafik"
                                                select new ScheduleForm
                                                {
                                                    Imie_Nazwisko = os.Imie_Nazwisko,
                                                    StartTime = data.StartTime,
                                                    EndTime = data.EndTime,
                                                    Dzial = dzial.DZL_Kod,
                                                    Lokalizacja = lokalizacja.Wartosc,
                                                    Description = data.Description,
                                                    RecurrenceRule = data.RecurrenceRule,
                                                }).ToList();

                    exportDatas.AddRange(datas);
                }

                ExportOptions options = new ExportOptions()
                {
                    ExportType = ExcelFormat.Xlsx,
                    CustomData = exportDatas,
                    FieldsInfo = exportFields,
                    Fields = new string[] { "Dzial", "Imie_Nazwisko", "Lokalizacja", "Description", "StartTime", "EndTime" },
                    DateFormat = this.DateFormat + " " + this.TimeFormat,
                    FileName = "Grafik na dzień " + DateTime.UtcNow.ToLocalTime(),
                    IncludeOccurrences = true,
                };
                await this.ScheduleRef.ExportToExcelAsync(options);
            }
            else if (args.Item.Text == "Wnioski")
            {
                foreach (var osoba in Osoby)
                {
                    List<ScheduleForm> datas = (from data in eventCollection
                                                join os in Osoby on data.PRI_PraId equals os.PRI_PraId
                                                join dzial in Dzialy on data.DZL_DzlId equals dzial.DZL_DzlId
                                                join wniosek in WnioskiData on data.RequestId equals wniosek.Id
                                                where os.PRI_PraId == osoba.PRI_PraId && data.Type == "Wniosek"
                                                select new ScheduleForm
                                                {
                                                    Imie_Nazwisko = os.Imie_Nazwisko,
                                                    StartTime = data.StartTime.Date,
                                                    EndTime = data.EndTime.AddDays(-1),
                                                    Dzial = dzial.DZL_Kod,
                                                    Wniosek = wniosek.Wartosc,
                                                    Description = data.Description,
                                                }).ToList();

                    exportDatas.AddRange(datas);
                }
                ExportOptions options = new ExportOptions()
                {
                    ExportType = ExcelFormat.Xlsx,
                    CustomData = exportDatas,
                    FieldsInfo = exportFields,
                    Fields = new string[] { "Dzial", "Imie_Nazwisko", "Wniosek", "Description", "StartTime", "EndTime" },
                    DateFormat = this.DateFormat /*+ " " + this.TimeFormat*/,
                    FileName = "Wnioski na dzień " + DateTime.UtcNow.ToLocalTime(),
                };
                await this.ScheduleRef.ExportToExcelAsync(options);
            }
            else
            {
                await this.ScheduleRef.ExportToICalendarAsync();
            }
        }

        public async Task OnOpen(BeforeOpenCloseMenuEventArgs<MenuItem> args)
        {
            if (args.ParentItem == null)
            {
                this.CellData = await this.ScheduleRef.GetTargetCellAsync((int)args.Left, (int)args.Top);
                await this.ScheduleRef.CloseQuickInfoPopupAsync();
                if (this.CellData == null)
                {
                    this.EventData = await this.ScheduleRef.GetTargetEventAsync((int)args.Left, (int)args.Top);
                    if (this.EventData.Id == Guid.Empty)
                    {
                        args.Cancel = true;
                    }

                    if (this.EventData.RecurrenceRule != null)
                    {
                        this.isCell = this.isEvent = true;
                        this.isRecurrence = false;
                    }
                    else
                    {
                        this.isCell = this.isRecurrence = true;
                        this.isEvent = false;
                    }
                }
                else
                {
                    this.isCell = false;
                    this.isEvent = this.isRecurrence = true;
                }
            }
        }

        public async Task OnItemSelected(MenuEventArgs<MenuItem> args)
        {
            var selectedMenuItem = args.Item.Id;
            var activeCellsData = await this.ScheduleRef.GetSelectedCellsAsync();
            if (activeCellsData == null)
            {
                activeCellsData = this.CellData;
            }

            switch (selectedMenuItem)
            {
                case "Today":
                    string key = this.TimezoneData.Key ?? "UTC";
                    this.SelectedDate = this.TimeConvertor(key);
                    break;
                case "Add":
                    await this.ScheduleRef.OpenEditorAsync(activeCellsData, CurrentAction.Add);
                    break;
                case "AddRecurrence":
                    ScheduleForm recurrenceEventData = null;
                    var resourceDetails = this.ScheduleRef.GetResourceByIndex(activeCellsData.GroupIndex);
                    recurrenceEventData = new ScheduleForm
                    {
                        Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                        StartTime = activeCellsData.StartTime,
                        EndTime = activeCellsData.StartTime,
                        IsAllDay = false,
                        PRI_PraId = resourceDetails.GroupData.PRI_PraId,
                        DZL_DzlId = resourceDetails.GroupData.DZL_DzlId,
                        RecurrenceRule = "FREQ=DAILY;INTERVAL=1;",
                    };
                    await this.ScheduleRef.OpenEditorAsync(recurrenceEventData, CurrentAction.Add);
                    break;
                case "Save":
                    await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.Save);
                    break;
                case "EditOccurrence":
                    await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.EditOccurrence);
                    break;
                case "EditSeries":
                    List<ScheduleForm> events = await this.ScheduleRef.GetEventsAsync();
                    this.EventData = (ScheduleForm)events.Where(data => data.Id == this.EventData.RecurrenceID).FirstOrDefault();
                    await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.EditSeries);
                    break;
                case "Delete":
                    await this.ScheduleRef.DeleteEventAsync(this.EventData);
                    break;
                case "DeleteOccurrence":
                    await this.ScheduleRef.DeleteEventAsync(this.EventData, CurrentAction.DeleteOccurrence);
                    break;
                case "DeleteSeries":
                    await this.ScheduleRef.DeleteEventAsync(this.EventData, CurrentAction.DeleteSeries);
                    break;
            }
        }

        private async void OnSettingsClick()
        {
            this.IsSettingsVisible = !this.IsSettingsVisible;
            this.StateHasChanged();
            await this.ScheduleRef.RefreshEventsAsync();
        }
    }
}
