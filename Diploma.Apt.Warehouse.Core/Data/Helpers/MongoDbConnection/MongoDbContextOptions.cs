using System;
using System.Collections.Generic;

namespace Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection
{
    public class MongoDbContextOptions
    {
        private string _mongoDbConnectionString;

        /// <summary>
        /// Mongodb connection string.
        /// </summary>
        public string MongoDbConnectionString
        {
            get => _mongoDbConnectionString;
            set => _mongoDbConnectionString = value;
        }

        private IDictionary<string, string> _mapping;

        public IDictionary<string, string> Mapping
        {
            get => _mapping;
            set => _mapping = new Dictionary<string, string>(value ?? new Dictionary<string, string>(), StringComparer.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// <see cref="MongoDbContextOptions" />.
    /// </summary>
    /// <typeparam name="TContext"> The type of the context these options apply to. </typeparam>
    public class MongoDbContextOptions<TContext> : MongoDbContextOptions where TContext : MongoDbContext
    {
        
    }
}