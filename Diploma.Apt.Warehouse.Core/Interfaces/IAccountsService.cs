using System;
using System.Threading.Tasks;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;

namespace Diploma.Apt.Warehouse.Core.Interfaces
{
    public interface IAccountsService
    {
        Task<AuthResponseModel> AuthenticateAsync(AuthRequestModel model);
        Task<bool> IsAdministratorAsync(Guid currentUserId);
        Task ConfirmOneAsync(Guid changeUserId);
    }
}