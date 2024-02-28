using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SoftlandERGrafik.Data.Entities.Forms;
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
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SoftlandERPGrafik.Web.Components.Pages
{
    public partial class Grafik
    {
        private IEnumerable<ADUser>? aduser;
        private UserPrincipal? userDetails;
        private bool IsDzialyExpanded = true;
        private Query DepartmanetQuery { get; set; } = new Query();
        private Query ResourceQuery { get; set; } = new Query();
        private Query LocalizationQuery { get; set; } = new Query();
        SfTextBox SubjectRef;
        SfMultiSelect<int[], ZatrudnieniDzialy> DepartamentRef;
        SfMultiSelect<int[], OsobaData> ResourceRef;
        SfMultiSelect<int[], OrganizacjaLokalizacje> LocationRef;
        SfSchedule<GrafikForm> ScheduleRef;
        bool isCreated;
        private Timezone TimezoneData { get; set; } = new Timezone();
        private static IEnumerable<OsobaData> Osoby;
        private static IEnumerable<ZatrudnieniDzialy> Dzialy;
        private IEnumerable<Kierownicy> Kierownik;
        private IEnumerable<OrganizacjaLokalizacje> LocalizationData;
        private List<string> ogolneStany = new List<string>();
        private List<GrafikForm> gridDataSource = new List<GrafikForm>();
        private bool ShowSchedule { get; set; } = true;
        private string SearchValue { get; set; }
        public GrafikForm EventData { get; set; }
        public CellClickEventArgs CellData { get; set; }
        private bool isCell { get; set; }
        private bool isEvent { get; set; }
        private bool isRecurrence { get; set; }
        private int SlotCount { get; set; } = 2;
        private int SlotInterval { get; set; } = 60;
        private int FirstDayOfWeek { get; set; } = 1;
        private bool TooltipEnable { get; set; } = true;
        private bool isRowAutoHeight { get; set; } = false;
        private bool EnableTimeScale { get; set; } = false;
        private bool isQuickInfoCreated { get; set; } = false;
        private View CurrentView { get; set; } = View.TimelineMonth;
        private string TimeFormat { get; set; } = "HH:mm";
        private bool IsSettingsVisible { get; set; } = false;
        public string[] GroupData = new string[] { "Dzialy", "Osoby" };
        private DateTime SystemTime { get; set; } = DateTime.UtcNow.ToLocalTime();
        private DateTime SelectedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        private bool disableState;

        protected override async Task OnInitializedAsync()
        {
            this.aduser = this.ADRepository.GetAllADUsers();
            Osoby = await this.GrafikService.GetEmployeesAsync();
            Dzialy = await this.GrafikService.GetDzialyAsync();
            this.TimezoneData = new Timezone().GetSystemTimeZone();
            this.LocalizationData = await this.GrafikService.GetLocalizationAsync();
            this.userDetails = await this.UserDetailsService.GetUserAllDetailsAsync();
            //this.Kierownik = await Kierownicy.GetAllAsync();
            var allStates = await StanVocabulary.GetAllAsync();
            this.ogolneStany = allStates.Where(s => (s as OgolneStan)?.Stan == "Aktywny").OrderBy(s => (s as OgolneStan)?.Wartosc).Select(s => (s as OgolneStan)?.Wartosc).ToList();
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
            var priOpis = Osoby.FirstOrDefault(o => o.PRI_PraId == priPraId)?.PRI_Opis;
            return this.userDetails?.SamAccountName == priOpis;
        }

        //private int? DepartmentManagerCRUD()
        //{
        //    bool isManager = Kierownik.Any(o => o.PRI_Opis == userDetails?.SamAccountName);

        //    if (isManager)
        //    {
        //        var priOpis = Kierownik.FirstOrDefault(o => o.PRI_Opis == userDetails?.SamAccountName)?.CNT_Nazwa;
        //        int? idDzial = Dzialy.FirstOrDefault(o => string.Equals(o.DZL_Kod, priOpis, StringComparison.OrdinalIgnoreCase)).DZL_DzlId;

        //        return idDzial;
        //    }

        //    return null;
        //}

        private OsobaData GetOsobaBySamAccountName(string samAccountName)
        {
            // Ensure that Osoby is not null before trying to find a person
            if (Osoby != null)
            {
                // Use LINQ to find the person with the matching SamAccountName
                return Osoby.FirstOrDefault(osoba => osoba.PRI_Opis == samAccountName);
            }

            // Handle the case where Osoby is null (you might want to throw an exception or log a message)
            return null;
        }

        // Style
        public async Task OnEventRendered(EventRenderedArgs<GrafikForm> args)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            string backgroundColor = args.Data.Color;
            string borderColor = "border-color: rgba(42, 65, 111, 0.2)";
            string backgroundImage = args.Data.Style;

            attributes.Add("style", $"background:{backgroundColor}; border-color:{borderColor}; background-image:{backgroundImage}");
            args.Attributes = attributes;
        }

        // Wyszukiwarka
        private T GetDataById<T>(int id, IEnumerable<T> data, Func<T, int> idSelector, Func<T> createDefault)
        {
            var item = data.FirstOrDefault(d => idSelector(d) == id);
            return item ?? createDefault();
        }

        private ZatrudnieniDzialy GetDzialDataByDzlId(int id)
        {
            return GetDataById(id, Dzialy, d => d.DZL_DzlId, () => new ZatrudnieniDzialy());
        }

        private OsobaData GetOsobaDataByDzlId(int id)
        {
            return GetDataById(id, Osoby, d => d.PRI_PraId, () => new OsobaData(new ZatrudnieniZrodlo()));
        }

        private OrganizacjaLokalizacje GetLocationDataByLokId(int id)
        {
            return GetDataById(id, LocalizationData, d => d.Lok_LokId, () => new OrganizacjaLokalizacje());
        }

        public async Task OnEventSearch()
        {
            if (!string.IsNullOrEmpty(SearchValue) && ScheduleRef != null)
            {
                Query query = new Query().Search(SearchValue, new List<string> { "Description" }, null, true, true);
                List<GrafikForm> eventCollections = await ScheduleRef.GetEventsAsync(null, null, true);
                object data = await new DataManager() { Json = eventCollections }.ExecuteQuery<GrafikForm>(query);
                List<GrafikForm> resultData = (data as List<GrafikForm>);
                if (resultData.Count > 0)
                {
                    ShowSchedule = false;
                    gridDataSource = resultData;
                }
                else
                {
                    ShowSchedule = true;
                    Snackbar.Add("Brak wyników wyszukiwania", Severity.Error);
                }
            }
            else
            {
                ShowSchedule = true;
            }
        }

        public void OnDepartamentChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<int[]> args)
        {
            WhereFilter predicate;

            if (args.Value != null && args.Value.Length > 0)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = "DZL_DzlId",
                    Operator = "equal",
                    value = id
                });

                // Łączymy filtry operatorem "lub" (Or).
                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));
                this.ResourceQuery = new Query().Where(predicate);
            }
            else
            {
                this.ResourceQuery = new Query();
            }
        }

        public void OnResourceChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<int[]> args)
        {
            WhereFilter predicate;

            if (args.Value != null && args.Value.Length > 0)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = "PRI_PraId",
                    Operator = "equal",
                    value = id
                });

                // Łączymy filtry operatorem "lub" (Or).
                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));
                this.ResourceQuery = new Query().Where(predicate);
            }
            else
            {
                this.ResourceQuery = new Query();
            }
        }

        public void OnLocalizationChange(Syncfusion.Blazor.DropDowns.MultiSelectChangeEventArgs<int[]> args)
        {
            WhereFilter predicate;

            if (args.Value != null && args.Value.Length > 0)
            {
                var filters = args.Value.Select(id => new WhereFilter
                {
                    Field = "LocationId",
                    Operator = "equal",
                    value = id
                });

                // Łączymy filtry operatorem "lub" (Or).
                predicate = filters.Aggregate((filter1, filter2) => filter1.Or(filter2));
                //this.LocalizationQuery = new Query().Where(predicate);
                this.LocalizationQuery = new Query().AddParams("LocationId", args.Value);
            }
            else
            {
                this.LocalizationQuery = new Query();

            }
        }

        private async void OnNewEventAdd()
        {
            DateTime Date = this.ScheduleRef.SelectedDate;
            DateTime Start = new DateTime(Date.Year, Date.Month, Date.Day, DateTime.Now.Hour, 0, 0);
            var samaccountname = this.userDetails?.SamAccountName;
            OsobaData user = this.GetOsobaBySamAccountName(samaccountname);
            GrafikForm eventData = new GrafikForm
            {
                Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                StartTime = Start,
                EndTime = Start.AddHours(1),
                IsAllDay = false,
                PRI_PraId = user.PRI_PraId,
                DZL_DzlId = user.DZL_DzlId,
            };
            await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
        }

        private async void OnNewRecurringEventAdd()
        {
            DateTime Date = this.ScheduleRef.SelectedDate;
            DateTime Start = new DateTime(Date.Year, Date.Month, Date.Day, DateTime.Now.Hour, 0, 0);
            var samaccountname = this.userDetails?.SamAccountName;
            OsobaData user = this.GetOsobaBySamAccountName(samaccountname);
            GrafikForm eventData = new GrafikForm
            {
                Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                StartTime = Start,
                EndTime = Start.AddHours(1),
                IsAllDay = false,
                PRI_PraId = user.PRI_PraId,
                DZL_DzlId = user.DZL_DzlId,
                RecurrenceRule = "FREQ=DAILY;INTERVAL=1;",
            };
            await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
        }

        private async void OnMoreDetailsClick(MouseEventArgs args, GrafikForm data, bool isEventData)
        {
            await ScheduleRef.CloseQuickInfoPopupAsync();
            GrafikForm eventData = new GrafikForm
            {
                Id = data.Id,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                LocationId = data.LocationId,
                Description = data.Description,
                IsAllDay = data.IsAllDay,
                PRI_PraId = data.PRI_PraId,
                DZL_DzlId = data.DZL_DzlId,
                RecurrenceException = data.RecurrenceException,
                RecurrenceID = data.RecurrenceID,
                RecurrenceRule = data.RecurrenceRule,
                Stan = data.Stan,
                Status = data.Status
            };

            if (isEventData == false)
            {
                await ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
            }
            else
            {
                await ScheduleRef.OpenEditorAsync(eventData, !string.IsNullOrEmpty(eventData.RecurrenceRule) ? CurrentAction.EditOccurrence : CurrentAction.Save);
            }
        }

        private async Task OnDelete(GrafikForm data)
        {
            await ScheduleRef.CloseQuickInfoPopupAsync();
            //await GrafikService.DeleteAppointment(data.Id);
            Snackbar.Add("Usunięto wpis", Severity.Warning);
            await ScheduleRef.RefreshEventsAsync();
        }

        private void BeforeOpenHandler(BeforeOpenCloseMenuEventArgs<MenuItem> e)
        {
            // While opening the first level context menu the parent item will not be available, so it would be null.
            if (e.ParentItem != null && e.ParentItem.Text == "View")
                disableState = !disableState; // Execute only for the View item sub menu.
        }

        private async Task OnToggleDzialyButton()
        {
            var dzialyIds = Dzialy.Select(d => d.DZL_DzlId).Distinct().ToList();

            foreach (var dzialId in dzialyIds)
            {
                if (IsDzialyExpanded)
                {
                    await this.ScheduleRef.CollapseResourceAsync(dzialId, "Dzialy");
                }
                else
                {
                    await this.ScheduleRef.ExpandResourceAsync(dzialId, "Dzialy");
                }
            }

            IsDzialyExpanded = !IsDzialyExpanded; // Odwracamy stan
        }

        private async Task PopupClose()
        {
            await ScheduleRef.CloseQuickInfoPopupAsync();
        }

        private void OnViewButtonClick(View targetView)
        {
            this.CurrentView = targetView;
        }

        private string GetEventDetails(GrafikForm data)
        {
            return data.StartTime.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture) + " (" + data.StartTime.ToString(TimeFormat, CultureInfo.CurrentCulture) + " - " + data.EndTime.ToString(TimeFormat, CultureInfo.CurrentCulture) + ")";
        }

        private async Task SetFocus()
        {
            if (isQuickInfoCreated)
            {
                await Task.Delay(20);
                await SubjectRef.FocusAsync();
            }
        }

        private async Task OnQuickInfoSubjectCreated()
        {
            await Task.Yield();
            await SubjectRef.FocusAsync();
            isQuickInfoCreated = true;
        }

        public void OnToolbarCreated()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>

            {
                string key = this.TimezoneData.Key ?? "UTC";
                SystemTime = this.TimeConvertor(key);
                ScheduleRef?.PreventRender();
                InvokeAsync(() => { StateHasChanged(); });
            });
            timer.Enabled = true;
        }

        private DateTime TimeConvertor(string TimeZoneId)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
        }

        public void OnTimeScaleChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            this.EnableTimeScale = args.Checked;
        }

        public void OnRowAutoHeightChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            this.isRowAutoHeight = args.Checked;
        }

        public async Task OnFileUploadChange(UploadChangeEventArgs args)
        {
            foreach (Syncfusion.Blazor.Inputs.UploadFiles file in args.Files)
            {
                StreamReader reader = new StreamReader(file.File.OpenReadStream(long.MaxValue));
                string fileContent = await reader.ReadToEndAsync();
                await ScheduleRef.ImportICalendarAsync(fileContent);
            }
        }

        public async void OnPrintClick()
        {
            await ScheduleRef.PrintAsync();
        }

        public async void OnExportClick(Syncfusion.Blazor.SplitButtons.MenuEventArgs args)
        {
            if (args.Item.Text == "Excel")
            {
                List<GrafikForm> ExportDatas = new List<GrafikForm>();
                List<GrafikForm> EventCollection = await ScheduleRef.GetEventsAsync();
                List<Syncfusion.Blazor.Schedule.Resource> ResourceCollection = ScheduleRef.GetResourceCollections();
                List<OsobaData> ResourceData = ResourceCollection[0].DataSource as List<OsobaData>;
                foreach (var osoba in Osoby)
                {
                    List<GrafikForm> datas = EventCollection
                        .Where(e => e.PRI_PraId == osoba.PRI_PraId)
                        .ToList();

                    ExportDatas.AddRange(datas);
                }

                ExportOptions Options = new ExportOptions()
                {
                    ExportType = ExcelFormat.Xlsx,
                    CustomData = ExportDatas,
                    Fields = new string[] { "PRI_PraId", "LocationId", "Description", "StartTime", "EndTime" },
                    DateFormat = "MM.dd.yyyy HH:mm:ss",
                    FileName = "Grafik na dzień:" + DateTime.UtcNow.ToLocalTime()
                };
                await ScheduleRef.ExportToExcelAsync(Options);
            }
            else
            {
                await ScheduleRef.ExportToICalendarAsync();
            }
        }

        public async Task OnOpen(BeforeOpenCloseMenuEventArgs<MenuItem> args)
        {
            if (args.ParentItem == null)
            {
                CellData = await ScheduleRef.GetTargetCellAsync((int)args.Left, (int)args.Top);
                await ScheduleRef.CloseQuickInfoPopupAsync();
                if (CellData == null)
                {
                    EventData = await ScheduleRef.GetTargetEventAsync((int)args.Left, (int)args.Top);
                    if (EventData.Id == null)
                    {
                        args.Cancel = true;
                    }

                    if (EventData.RecurrenceRule != null)
                    {
                        isCell = isEvent = true;
                        isRecurrence = false;
                    }
                    else
                    {
                        isCell = isRecurrence = true;
                        isEvent = false;
                    }
                }
                else
                {
                    isCell = false;
                    isEvent = isRecurrence = true;
                }
            }
        }

        public async Task OnItemSelected(MenuEventArgs<MenuItem> args)
        {
            var SelectedMenuItem = args.Item.Id;
            var ActiveCellsData = await ScheduleRef.GetSelectedCellsAsync();
            if (ActiveCellsData == null)
            {
                ActiveCellsData = CellData;
            }

            switch (SelectedMenuItem)
            {
                case "Today":
                    string key = this.TimezoneData.Key ?? "UTC";
                    SelectedDate = this.TimeConvertor(key);
                    break;
                case "Add":
                    await ScheduleRef.OpenEditorAsync(ActiveCellsData, CurrentAction.Add);
                    break;
                case "AddRecurrence":
                    GrafikForm RecurrenceEventData = null;
                    var resourceDetails = ScheduleRef.GetResourceByIndex(ActiveCellsData.GroupIndex);
                    RecurrenceEventData = new GrafikForm
                    {
                        Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                        StartTime = ActiveCellsData.StartTime,
                        EndTime = ActiveCellsData.StartTime.AddHours(1),
                        IsAllDay = false,
                        PRI_PraId = resourceDetails.GroupData.PRI_PraId,
                        DZL_DzlId = resourceDetails.GroupData.DZL_DzlId,
                        RecurrenceRule = "FREQ=DAILY;INTERVAL=1;",
                    };
                    await ScheduleRef.OpenEditorAsync(RecurrenceEventData, CurrentAction.Add);
                    break;
                case "Save":
                    await ScheduleRef.OpenEditorAsync(EventData, CurrentAction.Save);
                    break;
                case "EditOccurrence":
                    await ScheduleRef.OpenEditorAsync(EventData, CurrentAction.EditOccurrence);
                    break;
                case "EditSeries":
                    List<GrafikForm> Events = await ScheduleRef.GetEventsAsync();
                    EventData = (GrafikForm)Events.Where(data => data.RecurrenceID == EventData.RecurrenceID).FirstOrDefault();
                    await ScheduleRef.OpenEditorAsync(EventData, CurrentAction.EditSeries);
                    break;
                case "Delete":
                    await OnDelete(EventData);
                    break;
                case "DeleteOccurrence":
                    await ScheduleRef.DeleteEventAsync(EventData, CurrentAction.DeleteOccurrence);
                    break;
                case "DeleteSeries":
                    await ScheduleRef.DeleteEventAsync(EventData, CurrentAction.DeleteSeries);
                    break;
            }
        }

        private async void OnSettingsClick()
        {
            this.IsSettingsVisible = !this.IsSettingsVisible;
            StateHasChanged();
            await this.ScheduleRef.RefreshEventsAsync();
        }
    }
}
