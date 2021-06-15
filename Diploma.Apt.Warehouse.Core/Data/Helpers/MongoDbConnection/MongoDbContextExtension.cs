using System;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection
{
    public static class MongoDbContextExtension
    {
        /// <summary>
        /// Register new instance of <see cref="MongoDbContext" /> in service collection as a singleton.
        /// </summary>
        public static IServiceCollection AddDbContext<TContext>(
            this IServiceCollection collection,
            Action<MongoDbContextOptions<TContext>> setupAction)
            where TContext : MongoDbContext
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);
            return collection.AddSingleton<TContext, TContext>();
        }
    }
}