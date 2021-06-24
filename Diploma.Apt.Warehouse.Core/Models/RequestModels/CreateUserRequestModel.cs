using System;
using Diploma.Apt.Warehouse.Core.Models.DataModel;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class CreateUserRequestModel
    {
        public Guid? DepartmentId { get; set; }
        public string Password { get; set; }
        public UserDataModel Data { get; set; }
    }
}