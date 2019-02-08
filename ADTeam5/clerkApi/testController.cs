using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    public class testController : Controller
    {
        private readonly SignInManager<ADTeam5User> _signInManager;

        public testController(SignInManager<ADTeam5User> signInManager)
        {
            _signInManager = signInManager;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpGet("login/{email}/{password}")]
        public async Task<string> PostAsync(string email,string password)
        {
            //List<string> logininfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(logininfotemp);
            //String Email = logininfo[0];
            //String password = logininfo[1] ;
            var result = await _signInManager.PasswordSignInAsync(email, password,isPersistent:true, lockoutOnFailure: true);
            if (result.Succeeded)
                return "ok";
            else
                return "no";
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
