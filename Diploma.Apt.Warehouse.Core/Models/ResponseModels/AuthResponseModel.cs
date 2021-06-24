using System;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class AuthResponseModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }
        public string RoleType { get; set; }
    }
}