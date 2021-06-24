using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Services;
using MongoDB.Driver;

namespace Diploma.Apt.Warehouse.Core.Repositories
{
    public class UsersRepository : AbstractMongoRepository
    {
        private readonly IMongoCollection<UserEntity> _collection;
        private readonly IMapper _mapper;

        public UsersRepository(UserContext userContext, IMapper mapper) : base(userContext)
        {
            _mapper = mapper;
            _collection = userContext.Users;
        }

        public async Task<UserEntity> GetOneAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<UserEntity> GetOneByLoginAsync(string login)
        {
            return await _collection.Find(x => x.Data.Email == login || x.Data.Phone == login).FirstOrDefaultAsync();
        }

        public async Task<(bool, List<UserResponseModel>)> GetUsersByDepartmentIds(IEnumerable<Guid?> ids, int skip = 0, int limit = 50)
        {
            var result = await _collection.Find(Builders<UserEntity>.Filter.In(x => x.DepartmentId, ids))
                .Skip(skip)
                .Limit(limit + 1)
                .ToListAsync();
            var hasNext = result.Count > limit;
            result = result.Take(limit).ToList();
            return (hasNext, _mapper.Map<List<UserResponseModel>>(result));
        }
        
        public async Task ConfirmOneAsync(Guid id) =>
            await _collection.UpdateOneAsync(x => x.Id == id,
                Builders<UserEntity>.Update.Set(u => u.IsActive, true));

        public async Task CreateUser(CreateUserRequestModel model)
        {
            
            var existingUser = await GetOneByLoginAsync(model.Data.Email);
            if (existingUser != null) return;
            if (!string.IsNullOrEmpty(model.Data.Phone))
            {
                existingUser = await GetOneByLoginAsync(model.Data.Phone);
            }
            
            if(existingUser != null) return;
            var entity = _mapper.Map<UserEntity>(model);
            entity.IsActive = false;
            entity.Password = AccountsService.HashPassword(model.Password);
            await _collection.InsertOneAsync(entity);
        }
    }
}