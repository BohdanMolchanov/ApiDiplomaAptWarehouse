using System;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Diploma.Apt.Warehouse.Core.Services;
using Diploma.Apt.Warehouse.Core.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("register")]
    public class UserRegistrationController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly OrganizationsRepository _organizationsRepository;
        private readonly IMapper _mapper;
        
        
        public UserRegistrationController(UserContext userContext, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = new UsersRepository(userContext, mapper);
            _departmentsRepository = new DepartmentsRepository(userContext, mapper);
            _organizationsRepository = new OrganizationsRepository(userContext, mapper);
        }
        
        [HttpPost("organization")]
        public async Task<IActionResult> RegisterOrganization([FromBody] RegisterOrganizationRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var user = await _usersRepository.GetOneByLoginAsync(request.Data.Email);
            if (user == null)
            {
                var createUserRequest = _mapper.Map<CreateUserRequestModel>(request);
                createUserRequest.Data.RoleType = RoleTypes.Owner;
                await _usersRepository.CreateUser(createUserRequest);
                user = await _usersRepository.GetOneByLoginAsync(request.Data.Email);
            }
            if (user == null) return Conflict(new {message = "Failed to create user with such email"});
            var existingOrganization = await _organizationsRepository.GetOneByEdrpouAsync(request.Edrpou);
            if (existingOrganization != null) return Conflict(new { message = "This organization entity already exists"});

            var createOrganizationRequest = _mapper.Map<CreateOrganizationRequestModel>(request);
            createOrganizationRequest.OwnerId = user.Id;
            
            await _organizationsRepository.CreateOneAsync(createOrganizationRequest);
            var organization = await _organizationsRepository.GetOneByEdrpouAsync(request.Edrpou);
            if (organization == null) return Conflict(new {message = "Failed to create organization"});

            return Accepted();
        }

        [HttpPost("department"), Authorize(Roles = RoleTypes.Owner)]
        public async Task<IActionResult> RegisterDepartmentAsync([FromBody] CreateDepartmentRequestModel request)
        {
            if (request == null) return BadRequest();

            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var organization = await _organizationsRepository.GetOneByOwnerIdAsync(user.Id);
            if(organization == null) return Conflict(new { message = "This organization does not exist"});

            request.OrganizationId = organization.Id;
            await _departmentsRepository.CreateOneAsync(request);

            return Accepted();
        }

        [HttpPost("employee"), Authorize(Roles = RoleTypes.Owner)]
        public async Task<IActionResult> RegisterEmployeeAsync([FromBody] CreateUserRequestModel request)
        {
            var user = await _usersRepository.GetOneByLoginAsync(request.Data.Email);
            if (user != null) return Conflict(new { message = "User with such email already exists"});

            if (request.Data.RoleType != RoleTypes.Seller &&
                request.Data.RoleType != RoleTypes.WarehouseManager)
            {
                return Unauthorized(new {message = "Unable to register users with role {role}", request.Data.RoleType});
            }

            await _usersRepository.CreateUser(request);
            user = await _usersRepository.GetOneByLoginAsync(request.Data.Email);
            
            if (user == null) return Conflict(new { message = "User not created"});
            
            await _usersRepository.ConfirmOneAsync(user.Id);
            return Accepted();
        }
    }
}