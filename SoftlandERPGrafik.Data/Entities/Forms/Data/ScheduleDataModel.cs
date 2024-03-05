using SoftlandERPGrafik.Data.Entities.Staff.AD;
using SoftlandERPGrafik.Data.Entities.Views;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Schedule;
using System.DirectoryServices.AccountManagement;

namespace SoftlandERPGrafik.Data.Entities.Forms.Data
{
    public class ScheduleDataModel
    {
        public IEnumerable<ADUser>? aduser;
        public UserPrincipal? userDetails;
        public bool IsDzialyExpanded = true;
        public Query DepartmanetQuery { get; set; } = new Query();
        public Query ResourceQuery { get; set; } = new Query();
        public Query LocalizationQuery { get; set; } = new Query();
        public SfTextBox SubjectRef;
        public SfMultiSelect<int[], ZatrudnieniDzialy>? DepartamentRef;
        public SfMultiSelect<int[], OsobaData>? ResourceRef;
        public SfMultiSelect<int[], OrganizacjaLokalizacje>? LocationRef;
        public SfSchedule<WnioskiForm>? ScheduleRef;
        public bool isCreated;
        public Timezone TimezoneData { get; set; } = new Timezone();
        //public static IEnumerable<OsobaData>? Osoby;
        //public static IEnumerable<ZatrudnieniDzialy>? Dzialy;
        public IEnumerable<Kierownicy>? Kierownik;
        public IEnumerable<OrganizacjaLokalizacje>? LocalizationData;
        public List<string>? ogolneStany;
        public List<WnioskiForm>? gridDataSource;
        public bool ShowSchedule { get; set; } = true;
        public string SearchValue { get; set; }
        public WnioskiForm EventData { get; set; }
        public CellClickEventArgs CellData { get; set; }
        public bool isCell { get; set; }
        public bool isEvent { get; set; }
        public bool isRecurrence { get; set; }
        public int SlotCount { get; set; } = 2;
        public int SlotInterval { get; set; } = 60;
        public int FirstDayOfWeek { get; set; } = 1;
        public bool TooltipEnable { get; set; } = true;
        public bool isRowAutoHeight { get; set; } = false;
        public bool EnableTimeScale { get; set; } = false;
        public bool isQuickInfoCreated { get; set; } = false;
        public View CurrentView { get; set; } = View.TimelineMonth;
        public string TimeFormat { get; set; } = "HH:mm";
        public bool IsSettingsVisible { get; set; } = false;
        public string[] GroupData = new string[] { "Dzialy", "Osoby" };
        public DateTime SystemTime { get; set; } = DateTime.UtcNow.ToLocalTime();
        public DateTime SelectedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        public bool disableState = false;

    }
}
