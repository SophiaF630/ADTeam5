using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Areas.Identity.Pages.Account;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ADTeam5.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginApiController : ControllerBase
    {
        private readonly SignInManager<ADTeam5User> _signInManager;
        private ADTeam5UserContext _aDTeam5UserContext;
        private readonly ILogger<LoginModel> _logger;
        public LoginApiController(ADTeam5UserContext aDTeam5UserContext, SignInManager<ADTeam5User> signInManager, ILogger<LoginModel> logger)
        {
            _aDTeam5UserContext = aDTeam5UserContext;
            _signInManager = signInManager;
            _logger = logger;
        }
        [HttpPost]
        [Route("api/login/in")]
        public async Task<string> logInAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password,false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return "ok";
            }
            else
                return "no";
        }
    }
}