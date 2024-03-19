using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.DirectoryServices.AccountManagement;

namespace SoftlandERPGrafik.Web.Components.Adaptor
{
    public class DzialAdaptor : DataAdaptor
    {
        private readonly ScheduleService appService;
        private readonly UserDetailsService userDetailsService;
        private UserPrincipal? userDetails;

        public DzialAdaptor(ScheduleService appService, UserDetailsService userDetailsService)
        {
            this.appService = appService;
            this.userDetailsService = userDetailsService;
        }

        //Performs Read operation
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            System.Collections.Generic.IDictionary<string, object> @params = dataManagerRequest.Params;
            await Task.Delay(100);
            this.userDetails = await this.userDetailsService.GetUserAllDetailsAsync();
            var eventData = await this.appService.GetDepartamentAsync(this.userDetails?.SamAccountName);
            return dataManagerRequest.RequiresCounts ? new DataResult() : (object)eventData;
        }
    }
}
