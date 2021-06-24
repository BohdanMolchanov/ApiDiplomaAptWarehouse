using System;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class OrganizationResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Edrpou { get; set; }
        public Guid OwnerId { get; set; }
        public UserEntity Owner { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
    }
}