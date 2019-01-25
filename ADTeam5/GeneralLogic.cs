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


        //here comes an useless fuction
        public void updateCatalogue()
        {
            //this part will update the reorder level of catalogue;
            //this part will contain 5 parts:
            //first: we need to get the list of catalogue
            //second: for each catalogue we need to check if it has three purchase record
            //3: if do,then check if the 3rd time is quicker then 2nd time and quicker than 1st
            //then double the reorder level and reorder qty
            //4:if 3 is not,then check if the 3rd time is double then 2nd time
            //then half the reorder level and reorder qty
            //5:save data

            //this function will complete after the supplier part finished;
        }
    }
}
