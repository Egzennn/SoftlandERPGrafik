using SoftlandERGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace SoftlandERPGrafik.Web.Components.Adaptor
{
    public class GrafikAdaptor : DataAdaptor
    {
        private readonly GrafikService _appService;

        public GrafikAdaptor(GrafikService appService)
        {
            _appService = appService;
        }

        IEnumerable<GrafikForm>? EventData;

        //Performs Read operation
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            IDictionary<string, object> Params = dataManagerRequest.Params;
            DateTime start = DateTime.Parse((string)Params["StartDate"]);
            DateTime end = DateTime.Parse((string)Params["EndDate"]);
            EventData = await _appService.Get();
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = EventData, Count = EventData.Count() } : EventData;
        }

        //Performs Insert operation
        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            await _appService.Insert(data as GrafikForm);
            return data;
        }

        //Performs Update operation
        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            await _appService.Update(data as GrafikForm);
            return data;
        }

        //Performs Delete operation
        public async override Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key)
        {
            Guid id = (Guid)data;
            await _appService.Delete(id);
            return data;
        }

        //Performs Batch update operations
        public async override Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords, object addedRecords, object deletedRecords, string keyField, string key, int? dropIndex)
        {
            object records = deletedRecords;
            List<GrafikForm>? deleteData = deletedRecords as List<GrafikForm>;
            if (deleteData != null)
            {
                foreach (var data in deleteData)
                {
                    await _appService.Delete(data.Id);
                }
            }
            List<GrafikForm>? addData = addedRecords as List<GrafikForm>;
            if (addData != null)
            {
                foreach (var data in addData)
                {
                    await _appService.Insert(data as GrafikForm);
                    records = addedRecords;
                }
            }
            List<GrafikForm>? updateData = changedRecords as List<GrafikForm>;
            if (updateData != null)
            {
                foreach (var data in updateData)
                {
                    await _appService.Update(data as GrafikForm);
                    records = changedRecords;
                }
            }
            return records;
        }
    }
}
