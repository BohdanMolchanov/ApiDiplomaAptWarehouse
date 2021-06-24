using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL;
using Diploma.Apt.Warehouse.Core.Data.Helpers;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("admin"), Authorize(Roles = RoleTypes.Admin)]
    public class AdministratorController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly OrganizationsRepository _organizationsRepository;
        private readonly WarehouseContext _context;
        private readonly IMapper _mapper;
        
        public AdministratorController(UserContext userContext, IMapper mapper, WarehouseContext context)
        {
            _mapper = mapper;
            _context = context;
            _usersRepository = new UsersRepository(userContext, mapper);
            _departmentsRepository = new DepartmentsRepository(userContext, mapper);
            _organizationsRepository = new OrganizationsRepository(userContext, mapper);
        }
        
        [HttpGet("dictionary/products")]
        public async Task<IActionResult> GetProductsAsync(
            [FromQuery] int limit = 50, 
            [FromQuery] int skip = 0)
        {
            var query = _context.Products.AsNoTracking()
                .OrderBy(o => o.NameUkr)
                .ThenByDescending(t => t.TableKey)
                .Skip(skip)
                .Take(limit + 1);

            var queryResult = await query.ToListAsync();
            var hasNext = queryResult.Count > limit;
            
            queryResult = queryResult.Take(queryResult.Count - 1).ToList();

            var result = queryResult.Select(productEntity => new ProductResponseModel()
                {
                    Id = productEntity.Id,
                    Description = productEntity.Description,
                    CreatedAt = productEntity.CreatedAt.ToString("dd.MM.yyyy"),
                    NameEn = productEntity.NameEn,
                    NameUkr = productEntity.NameUkr,
                    ProductType = productEntity.ProductType
                })
                .ToList();

            return Ok(new ResponseData(result)
                {Meta = new Dictionary<string, object>{{"hasNext", hasNext}}});
        }

        [HttpPost("dictionary/products")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequestModel request)
        {
            if (request == null) return BadRequest();
            var existingProduct = await _context.Products.AsNoTracking()
                .Where(x => x.NameUkr == request.NameUkr && x.Description == request.Description)
                .FirstOrDefaultAsync();
            if (existingProduct != null) return Conflict(new {message = "Such product already exists"});
            var entity = _mapper.Map<ProductEntity>(request);
            entity.Id = Guid.NewGuid();
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Accepted();
        }

        [HttpGet("panel")]
        public async Task<IActionResult> GetAdminPanelAsync(
            [FromQuery] string search,
            [FromQuery] string extension,
            [FromQuery] bool? confirmed,
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            switch (extension)
            {
                case "organizations":
                {
                    var (hasNext, result) =
                        await _organizationsRepository.GetManyAsync(search, confirmed, skip, limit);

                    foreach (var organizationResponseModel in result)
                    {
                        organizationResponseModel.Owner =
                            await _usersRepository.GetOneAsync(organizationResponseModel.OwnerId);
                    }
                    
                    var response = new ResponseData(result)
                    {
                        Meta = new Dictionary<string, object>
                        {
                            {"hasNext", hasNext}
                        }
                    };
                    return Ok(response);
                }
                case "departments":
                {
                    var (hasNext, result) =
                        await _departmentsRepository.GetManyAsync(search, confirmed, skip, limit);
                    
                    foreach (var departmentResponseModel in result)
                    {
                        departmentResponseModel.Organization =
                            _mapper.Map<OrganizationResponseModel>(await _organizationsRepository.GetOneAsync(departmentResponseModel.OrganizationId));
                    }
                
                    var response = new ResponseData(result)
                    {
                        Meta = new Dictionary<string, object>
                        {
                            {"hasNext", hasNext}
                        }
                    };
                    return Ok(response);
                }
                default:
                    return BadRequest(new {message = "Extension must be chosen"});
            }
        }
        
        [HttpPatch("confirm/organization/{id:guid}")]
        public async Task<IActionResult> ConfirmOrganizationAsync([FromRoute] Guid id)
        {
            var organization = await _organizationsRepository.GetOneAsync(id);
            if (organization == null) return NotFound(new {message = "Organization with such Id not found"});
            var owner = await _usersRepository.GetOneAsync(organization.OwnerId);
            if (owner == null) return NotFound(new {message = "User with such Id not found"});

            await _organizationsRepository.ConfirmOneAsync(organization.Id);
            await _usersRepository.ConfirmOneAsync(owner.Id);

            return Ok();
        }
        
        [HttpPatch("confirm/department/{id:guid}")]
        public async Task<IActionResult> ConfirmDepartmentAsync([FromRoute] Guid id)
        {
            var department = await _departmentsRepository.GetOneAsync(id);

            if (department == null) return NotFound(new {message = "Department with such Id not found"});
            
            await _departmentsRepository.ConfirmOneAsync(department.Id);

            return Ok();
        }
        
        [HttpPatch("confirm/user/{id:guid}")]
        public async Task<IActionResult> ConfirmUserAsync([FromRoute] Guid id)
        {
            var user = await _usersRepository.GetOneAsync(id);

            if (user == null) return NotFound(new {message = "User with such Id not found"});
            
            await _usersRepository.ConfirmOneAsync(user.Id);

            return Ok();
        }
    }
}