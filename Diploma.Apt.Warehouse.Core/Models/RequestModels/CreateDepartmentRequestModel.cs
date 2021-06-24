using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class CreateDepartmentRequestModel
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string LocalityName { get; set; }
        public string AddressStreet { get; set; }
        public string AddressStreetNumber { get; set; }
    }
}