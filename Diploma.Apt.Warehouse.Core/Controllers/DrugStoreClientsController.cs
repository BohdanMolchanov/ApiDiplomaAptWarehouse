using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Helpers;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    public class DrugStoreClientsController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly OrganizationsRepository _organizationsRepository;
        private readonly WarehouseContext _context;
        private readonly IMapper _mapper;

        public DrugStoreClientsController(UserContext userContext, IMapper mapper, WarehouseContext context)
        {
            _usersRepository = new UsersRepository(userContext, mapper);
            _departmentsRepository = new DepartmentsRepository(userContext, mapper);
            _organizationsRepository = new OrganizationsRepository(userContext, mapper);
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("client/departments")]
        public async Task<IActionResult> GetDrugStores(
            [FromQuery] string drugName,
            [FromQuery] string region,
            [FromQuery] string area,
            [FromQuery] string localityName,
            [FromQuery] string addressStreet,
            [FromQuery] string addressStreetNumber,
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            var departmentIdsWithProducts = string.IsNullOrEmpty(drugName)
                ? new List<Guid>()
                : await _context.Batches.AsNoTracking()
                    .Include(x => x.Stock)
                    .ThenInclude(x => x.Product)
                    .Where(x =>
                        x.Stock.Product.SearchVector.Matches(
                            EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(drugName))
                        ) && x.Stock.Count > 0)
                    .Select(x => x.DepartmentId)
                    .Distinct()
                    .ToListAsync();
            var (hasNext, data) = await _departmentsRepository.GetManyByAddressAndIdsAsync(departmentIdsWithProducts,
                region, area, localityName, addressStreet,
                addressStreetNumber, skip, limit);

            foreach (var dep in data)
            {
                dep.Organization = await _organizationsRepository.GetOneResponseModelAsync(dep.OrganizationId);
            }

            var response = new ResponseData(data)
            {
                Meta = new Dictionary<string, object>
                {
                    {"hasNext", hasNext}
                }
            };
            return Ok(response);
        }

        [HttpGet("client/department/{id:guid}/information")]
        public async Task<IActionResult> GetDepartmentInformation(
            [FromRoute] Guid id, 
            [FromQuery] string search,
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            var department = await _departmentsRepository.GetOneAsync(id);
            if (department == null) return BadRequest();
            var result = _mapper.Map<DepartmentResponseModel>(department);
            result.Organization = await _organizationsRepository.GetOneResponseModelAsync(result.OrganizationId);
            var stockEntities = await _context.Stocks.AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.DepartmentId == id && x.Count > 0 &&
                            (string.IsNullOrEmpty(search) || x.Product.SearchVector.Matches(
                                EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(search))
                            ))
                )
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
            result.Products = stockEntities.Select(pe => new ProductResponseModel()
            {
                Description = pe.Product.Description,
                Id = pe.Product.Id,
                CreatedAt = pe.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy"),
                NameEn = pe.Product.NameEn,
                NameUkr = pe.Product.NameUkr,
                ProductType = pe.Product.ProductType,
                Price = pe.SellPrice.ToString(CultureInfo.InvariantCulture),
                TableKey = pe.TableKey
            }).ToList();
            
            return Ok(result);
        }
        
        [HttpGet("department/information"), Authorize(Roles = RoleTypes.WarehouseManager)]
        public async Task<IActionResult> GetDepartmentInformationForAuthorized(
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});
            var result = _mapper.Map<DepartmentResponseModel>(userDepartment);
            result.Organization = await _organizationsRepository.GetOneResponseModelAsync(result.OrganizationId);
            
            return Ok(result);
        }
    }
}