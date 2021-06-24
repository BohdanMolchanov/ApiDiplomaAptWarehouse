using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Interfaces;
using Diploma.Apt.Warehouse.Core.Models.DataModel;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Diploma.Apt.Warehouse.Core.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UsersRepository _usersRepository;
        
        public AccountsService(IMapper mapper, IConfiguration configuration, UserContext context)
        {
            _mapper = mapper;
            _configuration = configuration;
            _usersRepository = new UsersRepository(context, mapper);
        }

        public async Task<AuthResponseModel> AuthenticateAsync(AuthRequestModel model)
        {
            var user = await _usersRepository.GetOneByLoginAsync(model.Login);
            
            user = user?.Password == HashPassword(model.Password) ? user : null;

            // return null if user not found
            if (user == null)
                return null;

            if (!user.IsActive) return null;
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Name, user.Id.ToString()),
                    new (ClaimTypes.Role, user.Data.RoleType)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var response = _mapper.Map<AuthResponseModel>(user);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.Token = tokenHandler.WriteToken(token);
            return response;
        }

        public async Task<bool> IsAdministratorAsync(Guid currentUserId)
        {
            var currentUser = await _usersRepository.GetOneAsync(currentUserId);
            return currentUser.Data.RoleType == RoleTypes.Admin;
        }
        
        public async Task ConfirmOneAsync(Guid changeUserId)
        {
            await _usersRepository.ConfirmOneAsync(changeUserId);
        }
        
        #region utilities
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
        }
        #endregion
    }
}