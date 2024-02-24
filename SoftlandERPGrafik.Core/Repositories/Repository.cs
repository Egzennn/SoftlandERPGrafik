using Microsoft.EntityFrameworkCore;
using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.DB;

namespace SoftlandERPGrafik.Core.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly MainContext mainContext;
        protected readonly DbSet<T> table;

        public Repository(MainContext mainContext)
        {
            this.mainContext = mainContext;
            this.table = this.mainContext.Set<T>();
        }

        public async Task<IEnumerable<T>?> GetAllAsync()
        {
            try
            {
                return await this.table.ToListAsync().ConfigureAwait(true);
            }
            catch
            {
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(Guid? id)
        {
            try
            {
                var value = await this.table.FindAsync(id).ConfigureAwait(true);
                if (value != null)
                {
                    this.table.Entry(value).State = EntityState.Detached;
                }

                return value;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<T>> GetByListIdsAsync(List<Guid?> ids)
        {
            try
            {
                // Tworzy pustą listę wynikową.
                var resultList = new List<T>();

                // Iteruje przez listę identyfikatorów i pobiera rekordy dla każdego identyfikatora.
                foreach (var id in ids)
                {
                    if (id.HasValue)
                    {
                        var value = await this.table.FindAsync(id).ConfigureAwait(true);
                        if (value != null)
                        {
                            this.table.Entry(value).State = EntityState.Detached;
                            resultList.Add(value);
                        }
                    }
                }

                return resultList;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> InsertAsync(T? value)
        {
            try
            {
                if (value != null)
                {
                    await this.table.AddAsync(value).ConfigureAwait(true);
                    await this.mainContext.SaveChangesAsync().ConfigureAwait(true);
                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(T? value)
        {
            try
            {
                if (value != null)
                {
                    this.table.Attach(value);
                    this.table.Entry(value).State = EntityState.Modified;
                    await this.mainContext.SaveChangesAsync().ConfigureAwait(true);

                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateRecordsAsync<TEntity>(List<TEntity> recordsToUpdate)
        {
            using (var dbContext = this.mainContext)
            {
                foreach (var record in recordsToUpdate)
                {
                    dbContext.Entry(record).State = EntityState.Modified;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(Guid? id)
        {
            try
            {
                T? existing = await this.GetByIdAsync(id ?? default);

                if (existing != null)
                {
                    this.table.Remove(existing);
                    await this.mainContext.SaveChangesAsync().ConfigureAwait(true);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}