using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class CreateOrganizationRequestModel
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Edrpou { get; set; }
        public Guid OwnerId { get; set; }
    }
}