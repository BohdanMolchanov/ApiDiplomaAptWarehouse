using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    public class DepartmentEntity
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        [BsonElement("n")]
        public string Name { get; set; }
        [BsonElement("sn")]
        public string ShortName { get; set; }
        [BsonElement("oid")]
        public string OrganizationId { get; set; }
        [BsonElement("r")]
        public string Region { get; set; }
        [BsonElement("a")]
        public string Area { get; set; }
        [BsonElement("ln")]
        public string LocalityName { get; set; }
        [BsonElement("as")]
        public string AddressStreet { get; set; }
        [BsonElement("asn")]
        public string AddressStreetNumber { get; set; }
        [BsonElement("ia")]
        public bool IsActive { get; set; }
        [BsonElement("cat")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("uat"), BsonIgnoreIfNull]
        public DateTime? UpdatedAt { get; set; }
    }
}