using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixerDotNetCore.Components.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> All(FilterDefinition<T> filter);
        Task<T> Get(FilterDefinition<T> filter);
        Task<T> Add(T model);
        Task Add(List<T> models);
    }
}
