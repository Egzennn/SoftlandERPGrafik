namespace SoftlandERPGrafik.Core.Repositories.Interfaces
{
    public interface IRepository1<T>
        where T : class
    {
        Task<IEnumerable<T>?> GetAllAsync();

        Task<T?> GetByIdAsync(Guid? id);

        Task<List<T>> GetByListIdsAsync(List<Guid?> ids);

        Task<bool> InsertAsync(T? obj);

        Task<bool> UpdateAsync(T? obj);

        Task UpdateRecordsAsync<TEntity>(List<TEntity> recordsToUpdate);

        Task<bool> DeleteAsync(Guid? id);
    }
}