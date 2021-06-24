using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Diploma.Apt.Warehouse.Core.Data
{
    public class UserContext : MongoDbContext
    {
        public IMongoCollection<UserEntity> Users { get; set; }
        public IMongoCollection<OrganizationEntity> Organizations { get; set; }
        public IMongoCollection<DepartmentEntity> Departments { get; set; }

        public UserContext(IOptions<MongoDbContextOptions<UserContext>> options) : base(options)
        {
        }

        public override Task MigrateAsync()
            => Task.WhenAll(
                new List<Task>
                {
                    Users.Indexes.CreateManyAsync(
                        new[]
                        {
                            new CreateIndexModel<UserEntity>
                            (
                                Builders<UserEntity>.IndexKeys.Descending(x => x.DepartmentId),
                                new CreateIndexOptions {Background = true}),
                            new CreateIndexModel<UserEntity>
                            (
                                Builders<UserEntity>.IndexKeys.Ascending(x => x.Data.Email),
                                new CreateIndexOptions {Background = true}),
                            new CreateIndexModel<UserEntity>
                            (
                                Builders<UserEntity>.IndexKeys.Descending(x => x.Data.Phone),
                                new CreateIndexOptions {Background = true}),
                        }
                    ),
                    Departments.Indexes.CreateManyAsync(
                        new[]
                        {
                            new CreateIndexModel<DepartmentEntity>
                            (
                                Builders<DepartmentEntity>.IndexKeys.Combine(
                                    new BsonDocumentIndexKeysDefinition<DepartmentEntity>
                                    (
                                        new BsonDocument("r", 1)
                                    ),
                                    new BsonDocumentIndexKeysDefinition<DepartmentEntity>
                                    (
                                        new BsonDocument("a", 1)
                                    ),
                                    new BsonDocumentIndexKeysDefinition<DepartmentEntity>
                                    (
                                        new BsonDocument("ln", 1)
                                    ),
                                    new BsonDocumentIndexKeysDefinition<DepartmentEntity>
                                    (
                                        new BsonDocument("as", 1)
                                    )
                                ), new CreateIndexOptions {Name = "address_1", Background = true}),
                            new CreateIndexModel<DepartmentEntity>
                            (
                                Builders<DepartmentEntity>.IndexKeys.Ascending(x => x.IsActive),
                                new CreateIndexOptions {Background = true}),
                            new CreateIndexModel<DepartmentEntity>
                            (
                                Builders<DepartmentEntity>.IndexKeys.Descending(x => x.OrganizationId),
                                new CreateIndexOptions {Background = true}),
                        }),
                    Organizations.Indexes.CreateManyAsync(
                        new[]
                        {
                            new CreateIndexModel<OrganizationEntity>
                            (
                                Builders<OrganizationEntity>.IndexKeys.Ascending(x => x.IsActive),
                                new CreateIndexOptions {Background = true}),
                            new CreateIndexModel<OrganizationEntity>
                            (
                                Builders<OrganizationEntity>.IndexKeys.Ascending(x => x.Edrpou),
                                new CreateIndexOptions {Background = true})
                        })
                });
    }
}