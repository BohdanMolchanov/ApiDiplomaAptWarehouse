using Diploma.Apt.Warehouse.Core.Models.DataModel;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class RegisterOrganizationRequest
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Edrpou { get; set; }
        public string Password { get; set; }
        public UserDataModel Data { get; set; }
    }
}