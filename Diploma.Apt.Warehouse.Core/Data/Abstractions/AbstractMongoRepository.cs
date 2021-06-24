using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;
using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection;
using MongoDB.Driver;

namespace Diploma.Apt.Warehouse.Core.Repositories
{
    public abstract class AbstractMongoRepository
    {
        private readonly IMongoDatabase _mongodb;

        protected AbstractMongoRepository(MongoDbContext context)
        {
            _mongodb = context.Database;
        }
        
        internal async Task<bool> ExistsAsync<T>(Guid id) where T : AbstractMongoEntity, new()
        {
            var collection = _mongodb.GetCollection<T>(GetCollectionName<T>());
            return await collection.CountDocumentsAsync(d => d.Id == id).ContinueWith(r => r.Result > 0);
        }

        private static string GetCollectionName<T>() where T : class, new()
        {
            var model = new T();
            var collectionName = model.GetType().GetTypeInfo().GetCustomAttribute<TableAttribute>()
                ?.Name;
            return collectionName ?? model.GetType().Name.ToLower();
        }
    }
}