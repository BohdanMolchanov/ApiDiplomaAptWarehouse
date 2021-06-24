using System;
using System.Threading.Tasks;
using Diploma.Apt.Warehouse.Core.Interfaces;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        private readonly IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthRequestModel model)
        {
            var response = await _accountsService.AuthenticateAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }
    }
}