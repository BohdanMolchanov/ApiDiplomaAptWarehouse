using System;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Models.DataModel;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class UserResponseModel
    {
        public Guid Id { get; set; }
        public Guid? DepartmentId { get; set; }
        public DepartmentEntity Department { get; set; }
        public UserDataModel Data { get; set; }
        public string CreatedAt { get; set; }
    }
}