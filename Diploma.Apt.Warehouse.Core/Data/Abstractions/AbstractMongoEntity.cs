using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Abstractions
{
    public class AbstractMongoEntity
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [BsonElement("cat")]
        public DateTime? CreatedAt { get; set; } 

        [BsonElement("uat")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}