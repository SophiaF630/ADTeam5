using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5
{
    public class GeneralLogic
    {
        private readonly SSISTeam5Context _context;
        public GeneralLogic(SSISTeam5Context Context)
        {
            _context = Context;
        }
        public List<string> checkUserIdentityAsync(ADTeam5User user)
        {
            List<string> result = new List<string>();
            int workID = user.WorkID;
            //var users = from m in _context.User
            //             select m;

            var users = _context.User.FirstOrDefault(s => s.UserId == workID);
            if (users == null)
            {
                result.Add("NoWork");
                result.Add("NoJob");
                return result;
            }
            string department = users.DepartmentCode;
            result.Add(department);
            var roles = from m in _context.Department
                        select m;
            roles = roles.Where(s=>s.DepartmentCode == department);
            Department depm = roles.First();
            
            if(depm.DepartmentCode == "STAS")//this part should add the store code
            {
                if (depm.HeadId == workID)
                {
                    result.Add("Manager");
                }
                else if (depm.RepId == workID)
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
                if (depm.HeadId == workID)
                {
                    result.Add("Head");
                }
                else if (depm.RepId == workID)
                {
                    result.Add("Rep");
                }
                else if (depm.CoveringHeadId == workID)
                {
                    result.Add("CoveringHead");
                }
                else
                {
                    result.Add("Employee");
                }
            }
            return result;
        }
    }
}
