using System;
using System.ComponentModel.DataAnnotations.Schema;
using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    [Table("Organizations"), BsonIgnoreExtraElements]
    public class OrganizationEntity : AbstractMongoEntity
    {
        [BsonElement("n")]
        public string Name { get; set; }
        [BsonElement("sn")]
        public string ShortName { get; set; }
        [BsonElement("edr")]
        public string Edrpou { get; set; }
        [BsonElement("oid"), BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }
        [BsonIgnore]
        public UserEntity Owner { get; set; }
        [BsonElement("ia")]
        public bool IsActive { get; set; }
        
    }
}