using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace SoftlandERPGrafik.Web.Components.Adaptor
{
    public class WnioskiAdaptor : DataAdaptor
    {
        private readonly WnioskiService appService;

        public WnioskiAdaptor(WnioskiService appService)
        {
            this.appService = appService;
        }

        //Performs Read operation
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            System.Collections.Generic.IDictionary<string, object> @params = dataManagerRequest.Params;

            DateTime start = DateTime.Parse((string)@params["StartDate"]);
            DateTime end = DateTime.Parse((string)@params["EndDate"]);
            var eventData = await this.appService.Get(start, end);

            //if (@params.ContainsKey("LocationId") && @params["LocationId"] is IEnumerable<object> locationIds)
            //{
            //    List<int> locationIdList = locationIds.Select(id => Convert.ToInt32(id)).ToList();
            //    var eventDataLocation = eventData.Where(e => locationIdList.Contains(e.LocationId ?? 0)).ToList();

            //    return dataManagerRequest.RequiresCounts ? new DataResult() : eventDataLocation;
            //}
            //else
            //{
            return dataManagerRequest.RequiresCounts ? new DataResult() : eventData;
            //}
        }

        //Performs Insert operation
        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            await this.appService.Insert(data as WnioskiForm);
            return data;
        }

        //Performs Update operation
        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            await this.appService.Update(data as WnioskiForm);
            return data;
        }

        //Performs Delete operation
        public async override Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key)
        {
            Guid id = (Guid)data;
            await this.appService.Delete(id);
            return data;
        }

        //Performs Batch update operations
        public async override Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords, object addedRecords, object deletedRecords, string keyField, string key, int? dropIndex)
        {
            object records = deletedRecords;
            List<WnioskiForm>? deleteData = deletedRecords as List<WnioskiForm>;
            if (deleteData != null)
            {
                foreach (var data in deleteData)
                {
                    await this.appService.Delete(data.Id);
                }
            }

            List<WnioskiForm>? addData = addedRecords as List<WnioskiForm>;
            if (addData != null)
            {
                foreach (var data in addData)
                {
                    await this.appService.Insert(data as WnioskiForm);
                    records = addedRecords;
                }
            }

            List<WnioskiForm>? updateData = changedRecords as List<WnioskiForm>;
            if (updateData != null)
            {
                foreach (var data in updateData)
                {
                    await this.appService.Update(data as WnioskiForm);
                    records = changedRecords;
                }
            }

            return records;
        }
    }
}
