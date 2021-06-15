using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    [Table("Organizations"), BsonIgnoreExtraElements]
    public class OrganizationEntity
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        [BsonElement("n")]
        public string Name { get; set; }
        [BsonElement("sn")]
        public string ShortName { get; set; }
        [BsonElement("edr")]
        public string Edrpou { get; set; }
        [BsonElement("oid")]
        public string OwnerId { get; set; }
        [BsonIgnore]
        public UserEntity Owner { get; set; }
        [BsonElement("ia")]
        public bool IsActive { get; set; }
        [BsonElement("cat")]
        public DateTime? CreatedAt { get; set; }
        [BsonElement("uat"), BsonIgnoreIfNull]
        public DateTime? UpdatedAt { get; set; }
        
    }
}