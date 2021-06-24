using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Interfaces;
using Diploma.Apt.Warehouse.Core.Models.DataModel;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.Extensions.Configuration;

namespace Diploma.Apt.Warehouse.Core.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UsersRepository _usersRepository;

        public UserService(UserContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
            _usersRepository = new UsersRepository(context, mapper);
        }

        

        public async Task<UserEntity> GetOneAsync(Guid id)
        {
            return await _usersRepository.GetOneAsync(id);
        }

        
        
    }
}