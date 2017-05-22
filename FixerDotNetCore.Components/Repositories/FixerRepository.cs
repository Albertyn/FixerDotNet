using System.Threading.Tasks;
using FixerDotNetCore.Domain.Models;
using MongoDB.Driver;

namespace FixerDotNetCore.Components.Repositories
{
    public class FixerRepository : Repository<Fixer>, IFixerRepository
    {
        protected override string CollectionName { get; set; } = @"Fixes";

        public FixerRepository(IMongoConfiguration configuration) : base (configuration)
        { }

        public async Task<Fixer> GetLatestFixer(string Base)
        {
            var result = this._Database.GetCollection<Fixer>(this.CollectionName)
                .Find(x => x.@base.ToUpper() == Base.ToUpper())
                .SortByDescending(x => x.date).Limit(1);

            return await result.SingleAsync<Fixer>();
        }
    }
}
