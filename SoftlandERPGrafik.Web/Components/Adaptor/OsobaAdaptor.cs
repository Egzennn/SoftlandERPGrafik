using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace SoftlandERPGrafik.Web.Components.Adaptor
{
    public class OsobaAdaptor : DataAdaptor
    {
        private readonly ScheduleService appService;


        public OsobaAdaptor(ScheduleService appService)
        {
            this.appService = appService;
        }

        //Performs Read operation
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            System.Collections.Generic.IDictionary<string, object> @params = dataManagerRequest.Params;
            await Task.Delay(100);
            var eventData = await this.appService.GetEmployeesAsync();
            return dataManagerRequest.RequiresCounts ? new DataResult() : (object)eventData;
        }
    }
}
