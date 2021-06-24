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
    public class DepartmentsRepository : AbstractMongoRepository
    {
        private readonly IMongoCollection<DepartmentEntity> _collection;
        private readonly IMapper _mapper;

        public DepartmentsRepository(UserContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
            _collection = context.Departments;
        }

        public async Task<DepartmentEntity> GetOneAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<DepartmentEntity>> GetManyByIds(IEnumerable<Guid> ids)
        {
            var result = await _collection.Find(Builders<DepartmentEntity>.Filter.In(x => x.Id, ids))
                .ToListAsync();
            return result;
        }

        public async Task<(bool, List<DepartmentResponseModel>)> GetManyAsync(string search, bool? isActive, int skip = 0,
            int limit = 50)
        {
            var result = isActive.HasValue
                ? await _collection
                    .Find(x => x.IsActive == isActive && (string.IsNullOrEmpty(search) || 
                        (x.Name == search || x.ShortName == search)))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync()
                : await _collection
                    .Find(x => true && (string.IsNullOrEmpty(search) || 
                        (x.Name == search || x.ShortName == search)))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
            var hasNext = result.Count == limit;
            result = result.Take(limit).ToList();
            return (hasNext, _mapper.Map<List<DepartmentResponseModel>>(result));
        }
        public async Task<(bool, List<DepartmentResponseModel>)> GetManyByAddressAndIdsAsync(
            List<Guid> ids,
            string region,
            string area,
            string localityName,
            string addressStreet,
            string addressStreetNumber,
            int skip = 0,
            int limit = 50)
        {
            var filter = Builders<DepartmentEntity>.Filter.Eq(x => x.IsActive, true);
            if (!string.IsNullOrEmpty(region))
                filter &= Builders<DepartmentEntity>.Filter.Eq(x => x.Region, region);
            if (!string.IsNullOrEmpty(area))
                filter &= Builders<DepartmentEntity>.Filter.Eq(x => x.Area, area);
            if (!string.IsNullOrEmpty(localityName))
                filter &= Builders<DepartmentEntity>.Filter.Eq(x => x.LocalityName, localityName);
            if (!string.IsNullOrEmpty(addressStreet))
                filter &= Builders<DepartmentEntity>.Filter.Eq(x => x.AddressStreet, addressStreet);
            if (!string.IsNullOrEmpty(addressStreetNumber))
                filter &= Builders<DepartmentEntity>.Filter.Eq(x => x.AddressStreetNumber, addressStreetNumber);
            if (ids.Any())
                filter &= Builders<DepartmentEntity>.Filter.In(x => x.Id, ids);

            var result = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit + 1)
                .ToListAsync();
            var hasNext = result.Count > limit;
            result = result.Take(limit).ToList();
            return (hasNext, _mapper.Map<List<DepartmentResponseModel>>(result));
        }

        public async Task<(bool, List<DepartmentResponseModel>)> GetManyByOrganizationIdAsync(string search, Guid organizationId,
            int skip = 0, int limit = 50)
        {
            var result = await _collection
                .Find(x => x.OrganizationId == organizationId && (string.IsNullOrEmpty(search) || (x.Name == search || x.ShortName == search)))
                .Skip(skip)
                .Limit(limit + 1)
                .ToListAsync();
            var hasNext = result.Count > limit;
            result = result.Take(limit).ToList();
            return (hasNext, _mapper.Map<List<DepartmentResponseModel>>(result));
        }

        public async Task<List<DepartmentResponseModel>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            var result = await _collection
                .Find(x => x.OrganizationId == organizationId)
                .ToListAsync();
            return _mapper.Map<List<DepartmentResponseModel>>(result);
        }

        public async Task ConfirmOneAsync(Guid id) =>
            await _collection.UpdateOneAsync(x => x.Id == id,
                Builders<DepartmentEntity>.Update.Set(u => u.IsActive, true));

        public async Task CreateOneAsync(CreateDepartmentRequestModel model)
        {
            var entity = _mapper.Map<DepartmentEntity>(model);
            entity.IsActive = false;
            entity.CreatedAt = DateTime.Now;
            await _collection.InsertOneAsync(entity);
        }
    }
}