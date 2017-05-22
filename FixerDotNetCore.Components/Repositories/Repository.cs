using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FixerDotNetCore.Components.Repositories
{
    public abstract class Repository<T>:IRepository<T>
    {
        private readonly MongoUrl _Url;
        protected readonly MongoClient _Client;
        protected readonly IMongoDatabase _Database;

        protected abstract string CollectionName { get; set; }
        protected Repository(IMongoConfiguration configuration)
        {
            _Url = new MongoUrl(configuration.ConnectionString);
            _Client = new MongoClient(configuration.ConnectionString);
            _Database = _Client.GetDatabase(_Url.DatabaseName);
        }
        #region ALL
        public virtual async Task<List<T>> All(FilterDefinition<T> filter)
        {
            var collection = _Database.GetCollection<T>(CollectionName);
            var documents = await collection.FindAsync<T>(filter);
            return await documents.ToListAsync();
        }
        #endregion

        #region GET
        public virtual async Task<T> Get(FilterDefinition<T> filter)
        {
            var collection = _Database.GetCollection<T>(CollectionName);
            var document = await collection.FindAsync<T>(filter);
            return document.FirstOrDefault();
        }
        #endregion

        #region ADD
        public virtual async Task<T> Add(T document)
        {
            var collection = _Database.GetCollection<T>(CollectionName);
            await collection.InsertOneAsync(document);
            return document;
        }
        public virtual async Task Add(List<T> list)
        {
            var collection = _Database.GetCollection<T>(CollectionName);
            var writes = list.Select(doc => new InsertOneModel<T>(doc)).Cast<WriteModel<T>>().ToList();
            await collection.BulkWriteAsync(writes, new BulkWriteOptions { IsOrdered = false });
        }
        #endregion
    }
}
