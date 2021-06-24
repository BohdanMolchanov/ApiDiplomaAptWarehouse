using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;

namespace Diploma.Apt.Warehouse.Core.Interfaces
{
    public interface IUserService
    {
        
        Task<UserEntity> GetOneAsync(Guid id);
    }
}