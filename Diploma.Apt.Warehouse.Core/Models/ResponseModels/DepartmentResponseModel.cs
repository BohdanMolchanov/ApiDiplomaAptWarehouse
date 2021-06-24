using System;
using System.Collections.Generic;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class DepartmentResponseModel
    {
        public string AddressText => 
            $"{LocalityName}{(!string.IsNullOrEmpty(Region) ? $", {Region}" : "")}" +
            $"{(!string.IsNullOrEmpty(Area) ? $", {Area}" : "")}" +
            $"{(!string.IsNullOrEmpty(AddressStreet) ? $", {AddressStreet}" : "")}" +
            $"{(!string.IsNullOrEmpty(AddressStreetNumber) ? $", {AddressStreetNumber}" : "")}";
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Guid OrganizationId { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string LocalityName { get; set; } 
        public string AddressStreet { get; set; }
        public string AddressStreetNumber { get; set; }
        
        public bool IsActive { get; set; }
        public OrganizationResponseModel Organization { get; set; }
        public List<ProductResponseModel> Products { get; set; }
        public string CreatedAt { get; set; }
    }
}