using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class CollectionPasswordController : Controller
    {
        static int userid;
        static string dept;
        static string role;
        static string cp;

        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context context;
        DeptBizLogic b = new DeptBizLogic();
        readonly GeneralLogic userCheck;

        public CollectionPasswordController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GeneratePassword()
        {
            cp = b.generateCollectionPassword(dept);
            ViewData["CollectionPassword"] = cp;
            return View("Index");
        }
    }
}