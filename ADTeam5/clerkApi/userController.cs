using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADTeam5.clerkApi
{
    public partial class Account
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class userController : Controller
    {
        private readonly SignInManager<ADTeam5User> _signInManager;
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        public userController(UserManager<ADTeam5User> userManager, SSISTeam5Context context, SignInManager<ADTeam5User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("{id}")]
        public async Task<List<string>> GetAsync(int id)
        {
            List<string> result = new List<string>();
            var user = await _context.User
                .Include(u => u.DepartmentCodeNavigation)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return result;
            }
            else
            {
                //var depart = await _context.Department
                //.Include(d => d.DepartmentCodeNavigation)
                //.FirstOrDefaultAsync(m => m.UserId == id);
                result.Add(user.Name);
                result.Add(user.UserId.ToString());
                result.Add(user.DepartmentCodeNavigation.DepartmentName);
                result.Add(user.DepartmentCode);
                result.Add(user.EmailAddress);
                if (user.DepartmentCodeNavigation.DepartmentCode == "STAS")//this part should add the store code
                {
                    if (user.DepartmentCodeNavigation.HeadId == user.UserId)
                    {
                        result.Add("Manager");
                    }
                    else if (user.DepartmentCodeNavigation.RepId == user.UserId)
                    {
                        result.Add("Superviser");
                    }
                    else
                    {
                        result.Add("Clerk");
                    }
                }
                else
                {
                    if (user.DepartmentCodeNavigation.HeadId == user.UserId)
                    {
                        result.Add("Head");
                    }
                    else if (user.DepartmentCodeNavigation.RepId == user.UserId)
                    {
                        result.Add("Rep");
                    }
                    else if (user.DepartmentCodeNavigation.CoveringHeadId == user.UserId)
                    {
                        result.Add("CoveringHead");
                    }
                    else
                    {
                        result.Add("Employee");
                    }
                }
            }
            return result;
        }

        [HttpPost]
        public async Task<string> PostAsync(Account account)
        {
            //List<string> logininfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(logininfotemp);
            //String Email = logininfo[0];
            //String password = logininfo[1] ;
            var result = await _signInManager.PasswordSignInAsync(account.username, account.password, isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                User temp = _context.User.Where(s => s.EmailAddress == account.username).ToList().First();
                return temp.UserId.ToString();
            }
            else
                return "no";
        }
        [HttpGet("{username}/{password}")]
        public async Task<string> PostAsyncTest(string username,string password)
        {
            //List<string> logininfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(logininfotemp);
            //String Email = logininfo[0];
            //String password = logininfo[1] ;
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                User temp = _context.User.Where(s => s.EmailAddress == username).ToList().First();
                return temp.UserId.ToString();
            }
            else
                return null;
        }
        [HttpGet]
        public async Task<string> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.WorkID.ToString();
        }
        [HttpGet("checkpass/{id}/{password}")]
        public string CheckPass(string id,string password)
        {
            var dis = _context.DisbursementList.Where(s => s.Dlid == id).ToList().First();
            var dep = _context.Department.Where(s => s.DepartmentCode == dis.DepartmentCode).ToList().First();
            if (password == dep.CollectionPassword)
            {
                return "true";
            }
            else
                return "false";
        }
    }
}