using System;
using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    public class DepartmentEntity : AbstractMongoEntity
    {
        [BsonElement("n")]
        public string Name { get; set; }
        [BsonElement("sn")]
        public string ShortName { get; set; }
        [BsonElement("oid"), BsonRepresentation(BsonType.String)]
        public Guid OrganizationId { get; set; }
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
    }
}