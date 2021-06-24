using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("owner"), Authorize(Roles = RoleTypes.Owner)]
    public class OrganizationController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly OrganizationsRepository _organizationsRepository;
        private readonly IMapper _mapper;

        public OrganizationController(UserContext context, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = new UsersRepository(context, mapper);
            _departmentsRepository = new DepartmentsRepository(context, mapper);
            _organizationsRepository = new OrganizationsRepository(context, mapper);
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartmentsAsync(
            [FromQuery] string search,
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            var organization = await _organizationsRepository.GetOneByOwnerIdAsync(Guid.Parse(User.Identity.Name));
            if (organization == null) return BadRequest();
            var (hasNext, result) =
                await _departmentsRepository.GetManyByOrganizationIdAsync(search, organization.Id, skip, limit);
            
            var response = new ResponseData(result)
            {
                Meta = new Dictionary<string, object>
                {
                    {"hasNext", hasNext}
                }
            };
            return Ok(response);
        }
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployeesAsync(
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 50)
        {
            var organization = await _organizationsRepository.GetOneByOwnerIdAsync(Guid.Parse(User.Identity.Name));
            if (organization == null) return BadRequest();
            var departments = await _departmentsRepository.GetManyByOrganizationIdAsync(organization.Id);
            var departmentIds = departments.Select(x => x.Id);
            var (hasNext, result) = await _usersRepository.GetUsersByDepartmentIds(departmentIds as IEnumerable<Guid?>, skip, limit);
            
            var response = new ResponseData(result)
            {
                Meta = new Dictionary<string, object>
                {
                    {"hasNext", hasNext}
                }
            };
            return Ok(response);
        }

        [HttpGet("departments/list")]
        public async Task<IActionResult> GetDepartmentsListAsync()
        {
            var organization = await _organizationsRepository.GetOneByOwnerIdAsync(Guid.Parse(User.Identity.Name));
            var departments = await _departmentsRepository.GetManyByOrganizationIdAsync(organization.Id);
            var result = departments.Select(department => new DepartmentSearchResponseModel()
            {
                Name = $"{department.Name}, {department.LocalityName}",
                Id = department.Id
            });

            return Ok(result);
        }
        
    }
}