namespace FixerDotNetCore.Components.Repositories
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public MongoConfiguration() { }
        public string ConnectionString { get; }
        public MongoConfiguration(string connectionString) { ConnectionString = connectionString; }
    }
}
