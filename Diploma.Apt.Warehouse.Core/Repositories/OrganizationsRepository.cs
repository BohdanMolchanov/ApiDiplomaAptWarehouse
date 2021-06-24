using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using MongoDB.Driver;

namespace Diploma.Apt.Warehouse.Core.Repositories
{
    public class OrganizationsRepository : AbstractMongoRepository
    {
        private readonly IMongoCollection<OrganizationEntity> _collection;
        private readonly IMapper _mapper;

        public OrganizationsRepository(UserContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
            _collection = context.Organizations;
        }

        public async Task<OrganizationEntity> GetOneAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<OrganizationResponseModel> GetOneResponseModelAsync(Guid id)
        {
            var result = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<OrganizationResponseModel>(result);
        }

        public async Task<(bool, List<OrganizationResponseModel>)> GetManyAsync(string search, bool? isActive, int skip = 0,
            int limit = 50)
        {
            var result = isActive.HasValue
                ? await _collection
                    .Find(x => x.IsActive == isActive && (string.IsNullOrEmpty(search) || 
                        (x.Edrpou == search || x.Name == search || x.ShortName == search)))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync()
                : await _collection
                    .Find(x => true && (string.IsNullOrEmpty(search) || 
                        (x.Edrpou == search || x.Name == search || x.ShortName == search)))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
            var hasNext = result.Count == limit;
            result = result.Take(limit).ToList();
            return (hasNext, _mapper.Map<List<OrganizationResponseModel>>(result));
        }

        public async Task<OrganizationEntity> GetOneByOwnerIdAsync(Guid ownerId)
            => await _collection.Find(x => x.OwnerId == ownerId).FirstOrDefaultAsync();

        public async Task<OrganizationEntity> GetOneByEdrpouAsync(string edrpou)
        {
            return await _collection.Find(x => x.Edrpou == edrpou).FirstOrDefaultAsync();
        }

        public async Task ConfirmOneAsync(Guid id) =>
            await _collection.UpdateOneAsync(x => x.Id == id,
                Builders<OrganizationEntity>.Update.Set(u => u.IsActive, true));

        public async Task CreateOneAsync(CreateOrganizationRequestModel model)
        {
            var entity = _mapper.Map<OrganizationEntity>(model);
            entity.IsActive = false;
            entity.CreatedAt = DateTime.Now;
            await _collection.InsertOneAsync(entity);
        }
    }
}