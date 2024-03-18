using SoftlandERPGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace SoftlandERPGrafik.Web.Components.Adaptor
{
    public class ScheduleAdaptor : DataAdaptor
    {
        private readonly ScheduleService appService;

        public ScheduleAdaptor(ScheduleService appService)
        {
            this.appService = appService;
        }

        //Performs Read operation
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            System.Collections.Generic.IDictionary<string, object> @params = dataManagerRequest.Params;

            var eventData = await this.appService.Get();

            if (@params != null)
            {
                if (@params.ContainsKey("LocationId") && @params["LocationId"] is IEnumerable<object> locationIds)
                {
                    List<int> locationIdList = locationIds.Select(id => Convert.ToInt32(id)).ToList();
                    var eventDataByLocationId = eventData.Where(e => locationIdList.Contains(e.LocationId ?? 0)).ToList();

                    return dataManagerRequest.RequiresCounts ? new DataResult() : eventDataByLocationId;
                }
                else if (@params.ContainsKey("RequestId") && @params["RequestId"] is IEnumerable<object> requestIds)
                {
                    List<Guid> requestIdList = requestIds.Select(id => Guid.Parse(id.ToString())).ToList();
                    var eventDataFilteredByRequestId = eventData.Where(e => requestIdList.Contains(e.RequestId ?? Guid.Empty)).ToList();

                    return dataManagerRequest.RequiresCounts ? new DataResult() : eventDataFilteredByRequestId;
                }
                else if (@params.ContainsKey("Type") && @params["Type"] is IEnumerable<object> types)
                {
                    List<string?> typeList = types.Select(type => type.ToString()).ToList();
                    var eventDataFilteredByType = eventData.Where(e => typeList.Contains(e.Type)).ToList();

                    return dataManagerRequest.RequiresCounts ? new DataResult() : eventDataFilteredByType;
                }
            }

            // Jeśli żadne specjalne filtry nie są używane, zwróć wszystkie dane
            return dataManagerRequest.RequiresCounts ? new DataResult() : eventData;
        }

        //Performs Insert operation
        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            await this.appService.Insert(data as ScheduleForm);
            return data;
        }

        //Performs Update operation
        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            await this.appService.Update(data as ScheduleForm);
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
            List<ScheduleForm>? deleteData = deletedRecords as List<ScheduleForm>;
            if (deleteData != null)
            {
                foreach (var data in deleteData)
                {
                    await this.appService.Delete(data.Id);
                }
            }

            List<ScheduleForm>? addData = addedRecords as List<ScheduleForm>;
            if (addData != null)
            {
                foreach (var data in addData)
                {
                    await this.appService.Insert(data as ScheduleForm);
                    records = addedRecords;
                }
            }

            List<ScheduleForm>? updateData = changedRecords as List<ScheduleForm>;
            if (updateData != null)
            {
                foreach (var data in updateData)
                {
                    await this.appService.Update(data as ScheduleForm);
                    records = changedRecords;
                }
            }

            return records;
        }
    }
}
