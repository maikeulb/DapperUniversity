using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DapperUniversity.Data
{
    public class Query<T>
    {
        private readonly DbContext _dbContext;

        public Query(DbContext context)
        {
            _dbContext = context;
        }

        public async Task<int> Create(T item)
        {
            return await _dbContext.GetConnection().InsertAsync<int>(item);
        }

        public async Task<T> Read(int id)
        {
            return await _dbContext.GetConnection().GetAsync<T>(id);
        }

        public async Task<T> Read(object whereConditions)
        {
            return (await _dbContext.GetConnection().GetListAsync<T>(whereConditions)).First();
        }

        public async Task<T> ReadOrDefault(object whereConditions)
        {
            return (await _dbContext.GetConnection().GetListAsync<T>(whereConditions)).FirstOrDefault();
        }

        public async Task<bool> Update(T item)
        {
            var affectedRows = await _dbContext.GetConnection().UpdateAsync(item);
            return affectedRows >= 1;
        }

        public async Task<bool> Delete(T item)
        {
            var affectedRows = await _dbContext.GetConnection().DeleteAsync(item);
            return affectedRows >= 1;
        }

        public async Task<IEnumerable<T>> List()
        {
            return await _dbContext.GetConnection().GetListAsync<T>();
        }

        public async Task<IEnumerable<T>> List(object whereConditions)
        {
            return await _dbContext.GetConnection().GetListAsync<T>(whereConditions);
        }
    }
}
