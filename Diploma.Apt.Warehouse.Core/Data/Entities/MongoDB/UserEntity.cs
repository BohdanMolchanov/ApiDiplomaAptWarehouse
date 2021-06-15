using System;
using System.ComponentModel.DataAnnotations.Schema;
using Diploma.Apt.Warehouse.Core.Models.DataModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB
{
    [Table("Users"), BsonIgnoreExtraElements]
    public class UserEntity
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        [BsonElement("did"), BsonIgnoreIfNull]
        public string DepartmentId { get; set; }
        [BsonIgnore]
        public DepartmentEntity Department { get; set; }
        [BsonElement("p")]
        public string Password { get; set; }
        [BsonElement("d")]
        public UserDataModel UserDataModel { get; set; }
        [BsonElement("ia")]
        public bool IsActive { get; set; }
        [BsonElement("cat")]
        public DateTime? CreatedAt { get; set; }
        [BsonElement("uat"), BsonIgnoreIfNull]
        public DateTime? UpdatedAt { get; set; }
    }

    
}