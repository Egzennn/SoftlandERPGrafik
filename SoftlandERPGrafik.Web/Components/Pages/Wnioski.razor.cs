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
    using Syncfusion.Blazor.Data;
    using Syncfusion.Blazor.DropDowns;
    using Syncfusion.Blazor.Inputs;
    using Syncfusion.Blazor.Navigations;
    using Syncfusion.Blazor.Schedule;
    using Timer = System.Timers.Timer;

    public partial class Wnioski
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
        SfMultiSelect<int[], OrganizacjaLokalizacje>? LocationRef;
        SfSchedule<WnioskiForm>? ScheduleRef;
        bool isCreated;
        private Timezone TimezoneData { get; set; } = new Timezone();
        private static IEnumerable<OsobaData>? Osoby;
        private static IEnumerable<ZatrudnieniDzialy>? Dzialy;
        private IEnumerable<Kierownicy>? Kierownik;
        private IEnumerable<OrganizacjaLokalizacje>? LocalizationData;
        private static IEnumerable<OgolneWnioski>? WnioskiData;
        private List<string?> ogolneStatusy;
        private List<WnioskiForm>? gridDataSource;
        private bool ShowSchedule { get; set; } = true;
        private string SearchValue { get; set; }
        public WnioskiForm EventData { get; set; }
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
        private string DateFormat { get; set; } = "yyyy-MM-dd";
        private bool IsSettingsVisible { get; set; } = false;
        public string[] GroupData = new string[] { "Dzialy", "Osoby" };
        private DateTime SystemTime { get; set; } = DateTime.UtcNow.ToLocalTime();
        private DateTime SelectedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        private bool disableState = false;
        private bool enableStateA = false;
        private bool enableStateB = false;
        private List<string> SignedInGroup;

        protected override async Task OnInitializedAsync()
        {
            this.aduser = this.ADRepository.GetAllADUsers();
            Osoby = await this.WnioskiService.GetEmployeesAsync();
            Dzialy = await this.WnioskiService.GetDzialyAsync();
            this.TimezoneData = new Timezone().GetSystemTimeZone();
            this.userDetails = await this.UserDetailsService.GetUserAllDetailsAsync();
            this.Kierownik = await this.Kierownicy.GetAllAsync();
            this.ogolneStatusy = await this.WnioskiService.GetStatusAsync();
            WnioskiData = await this.WnioskiService.GetWnioskiAsync();
            this.SignedInGroup = this.WnioskiService.GetSignedInGroups(userDetails?.SamAccountName);
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
            var priOpis = Osoby.FirstOrDefault(o => o.PRI_PraId == priPraId)?.PRI_Opis;
            return this.userDetails?.SamAccountName == priOpis;
        }

        private int? DepartmentManagerCRUD()
        {
            bool isManager = this.Kierownik.Any(o => o.PRI_Opis == this.userDetails?.SamAccountName);

            if (isManager)
            {
                var priOpis = this.Kierownik.FirstOrDefault(o => o.PRI_Opis == this.userDetails?.SamAccountName)?.CNT_Nazwa;
                int idDzial = Dzialy.FirstOrDefault(o => string.Equals(o.DZL_Kod, priOpis, StringComparison.OrdinalIgnoreCase)).DZL_DzlId;

                return idDzial;
            }

            return null;
        }

        private OsobaData? GetOsobaBySamAccountName(string samAccountName)
        {
            if (Osoby != null)
            {
                return Osoby.FirstOrDefault(osoba => osoba.PRI_Opis == samAccountName);
            }

            return null;
        }

        // Style
        public async Task OnEventRendered(EventRenderedArgs<WnioskiForm> args)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            DateTime endTime = args.Data.EndTime;

            string backgroundColor = args.Data.Color;
            string borderColor = "border-color: rgba(42, 65, 111, 0.2)";
            string backgroundImage = args.Data.Style;

            attributes.Add("style", $"background:{backgroundColor}; border-color:{borderColor}; background-image:{backgroundImage}");
            args.Attributes = attributes;
            if (endTime <= this.SystemTime)
            {
                args.Data.IsReadonly = true;
                this.StateHasChanged();
            }
        }

        // Wyszukiwarka
        private T GetDataById<T>(int? id, IEnumerable<T> data, Func<T, int> idSelector, Func<T> createDefault)
        {
            var item = data.FirstOrDefault(d => idSelector(d) == id);
            return item ?? createDefault();
        }

        private OsobaData GetOsobaDataByDzlId(int id)
        {
            return this.GetDataById(id, Osoby, d => d.PRI_PraId, () => new OsobaData(new ZatrudnieniZrodlo()));
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

        private async void OnNewEventAdd()
        {
            DateTime date = this.ScheduleRef.SelectedDate;
            DateTime start = new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, 0, 0);
            var samaccountname = this.userDetails?.SamAccountName;
            OsobaData? user = this.GetOsobaBySamAccountName(samaccountname);
            WnioskiForm eventData = new WnioskiForm
            {
                Id = await this.ScheduleRef.GetMaxEventIdAsync<Guid>(),
                StartTime = start,
                EndTime = start.AddHours(1),
                IsAllDay = false,
                PRI_PraId = user.PRI_PraId,
                DZL_DzlId = user.DZL_DzlId,
            };
            await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
        }

        private async void OnMoreDetailsClick(MouseEventArgs args, WnioskiForm data, bool isEventData)
        {
            await this.ScheduleRef.CloseQuickInfoPopupAsync();
            WnioskiForm eventData = new WnioskiForm
            {
                Id = data.Id,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                RequestId = data.RequestId,
                Description = data.Description,
                IDD = data.IDD,
                IDS = data.IDS,
                IsAllDay = data.IsAllDay,
                PRI_PraId = data.PRI_PraId,
                DZL_DzlId = data.DZL_DzlId,
                Stan = data.Stan,
                Status = data.Status,
                CreatedBy = data.CreatedBy,
            };

            if (isEventData == false)
            {
                await this.ScheduleRef.OpenEditorAsync(eventData, CurrentAction.Add);
            }
            else
            {
                await this.ScheduleRef.OpenEditorAsync(eventData, !string.IsNullOrEmpty(eventData.RecurrenceRule) ? CurrentAction.EditOccurrence : CurrentAction.Save);
            }
        }

        private async Task OnDelete(WnioskiForm data)
        {
            await this.ScheduleRef.CloseQuickInfoPopupAsync();
            await this.WnioskiService.Delete(data.Id);
            this.Snackbar.Add("Usunięto wpis", Severity.Warning);
            await this.ScheduleRef.RefreshEventsAsync();
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
            this.CurrentView = targetView;
        }

        private string GetEventDetails(WnioskiForm data)
        {
            return data.StartTime.ToString(this.DateFormat, CultureInfo.CurrentCulture) + " - " + data.EndTime.AddDays(-1).ToString(this.DateFormat, CultureInfo.CurrentCulture);
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

        private void OnTimeScaleChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            this.EnableTimeScale = args.Checked;
        }

        private void OnRowAutoHeightChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            this.isRowAutoHeight = args.Checked;
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
            if (args.Item.Text == "Excel")
            {
                List<WnioskiForm> exportDatas = new List<WnioskiForm>();
                List<WnioskiForm> eventCollection = await this.ScheduleRef.GetEventsAsync();
                List<Syncfusion.Blazor.Schedule.Resource> resourceCollection = this.ScheduleRef.GetResourceCollections();
                List<OsobaData>? resourceData = resourceCollection[0].DataSource as List<OsobaData>;
                foreach (var osoba in Osoby)
                {
                    List<WnioskiForm> datas = eventCollection
                        .Where(e => e.PRI_PraId == osoba.PRI_PraId)
                        .ToList();

                    exportDatas.AddRange(datas);
                }

                ExportOptions Options = new ExportOptions()
                {
                    ExportType = ExcelFormat.Xlsx,
                    CustomData = exportDatas,
                    Fields = new string[] { "PRI_PraId", "LocationId", "Description", "StartTime", "EndTime" },
                    DateFormat = "MM.dd.yyyy HH:mm:ss",
                    FileName = "Grafik na dzień:" + DateTime.UtcNow.ToLocalTime(),
                };
                await this.ScheduleRef.ExportToExcelAsync(Options);
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
                this.CellData = await ScheduleRef.GetTargetCellAsync((int)args.Left, (int)args.Top);
                await this.ScheduleRef.CloseQuickInfoPopupAsync();
                if (this.CellData == null)
                {
                    this.EventData = await this.ScheduleRef.GetTargetEventAsync((int)args.Left, (int)args.Top);
                    if (this.EventData.Id == null)
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
                case "Save":
                    await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.Save);
                    break;
                case "EditOccurrence":
                    await this.ScheduleRef.OpenEditorAsync(this.EventData, CurrentAction.EditOccurrence);
                    break;
                case "Delete":
                    await this.ScheduleRef.DeleteEventAsync(this.EventData);
                    break;
                case "DeleteOccurrence":
                    await this.ScheduleRef.DeleteEventAsync(this.EventData, CurrentAction.DeleteOccurrence);
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
