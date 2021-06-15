using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection
{
    public abstract class MongoDbContext
    {
        /// <summary>
        /// Mongodb driver database.
        /// </summary>
        public IMongoDatabase Database { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbContext" /> class using the specified options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public MongoDbContext(IOptions<MongoDbContextOptions> options)
        {
            if(string.IsNullOrEmpty(options?.Value?.MongoDbConnectionString)) 
                throw new ArgumentNullException(nameof(options.Value.MongoDbConnectionString));
            
            Database = new MongoClient(options.Value.MongoDbConnectionString).GetDatabase(MongoUrl.Create(options.Value.MongoDbConnectionString).DatabaseName);
            
            var method = typeof(IMongoDatabase).GetMethod("GetCollection");
            foreach (var propertyInfo in this.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType 
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>)))
            {
                if(method == null) throw new NullReferenceException();

                if (!propertyInfo.CanWrite) continue;
                var collectionType = propertyInfo.PropertyType.GetGenericArguments()[0];
                
                var tableName = collectionType.GetCustomAttribute<TableAttribute>()?.Name ?? propertyInfo.Name;
                if (options.Value?.Mapping != null && options.Value.Mapping.TryGetValue(tableName, out var overrideTableName))
                    tableName = overrideTableName;

                propertyInfo.SetValue(this, 
                    method.MakeGenericMethod(collectionType)
                        .Invoke(Database, new object[]{tableName, null}));
                
                Console.WriteLine($"{propertyInfo.Name}=>{tableName}");
            }
        }

        /// <summary>
        /// Asynchronously applies migration code for the context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public virtual Task MigrateAsync() => Task.CompletedTask;

        /// <summary>
        /// Applies migration code for the context to the database.
        /// </summary>
        public virtual void Migrate() => MigrateAsync().Wait();
    }
}