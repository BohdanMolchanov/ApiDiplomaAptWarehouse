using System;
using System.ComponentModel.DataAnnotations.Schema;
using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using Diploma.Apt.Warehouse.Core.Models.DataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    [Table("Users"), BsonIgnoreExtraElements]
    public class UserEntity : AbstractMongoEntity
    {
        [BsonElement("did"), BsonIgnoreIfNull, BsonRepresentation(BsonType.String)]
        public Guid? DepartmentId { get; set; }
        [BsonIgnore]
        public DepartmentEntity Department { get; set; }
        [BsonElement("p")]
        public string Password { get; set; }
        [BsonElement("d")]
        public UserDataModel Data { get; set; }
        [BsonElement("ia")]
        public bool IsActive { get; set; }
    }

    
}