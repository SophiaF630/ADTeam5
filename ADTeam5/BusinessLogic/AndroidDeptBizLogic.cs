using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.BusinessLogic
{
    public class AndroidDeptBizLogic
    {
        private readonly SSISTeam5Context context = new SSISTeam5Context();

        public List<String> ListEmployeeNames(string dept)
        {
            var t = context.Department.Where(x => x.DepartmentCode == dept).First();
            int headid = t.HeadId;
            int repid = t.RepId;
            var q = context.User.Where(x => x.DepartmentCode == dept && x.UserId != headid && x.UserId != repid).ToList();
            List<String> list = new List<String>();
            foreach(User current in q)
            {
                list.Add(current.Name);
            }
            return list;
        }

        public BriefEmp GetRep(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int repid = q.RepId;
            var p = context.User.Where(x => x.UserId == repid).First();
            BriefEmp b = new BriefEmp();
            b.userId = p.UserId;
            b.name = p.Name;
            return b;
        }

        public List<BriefEmp> ListEmployeesBrief(string dept)
        {
            var t = context.Department.Where(x => x.DepartmentCode == dept).First();
            int headid = t.HeadId;
            int repid = t.RepId;

            var q = from x in context.User
                    where x.DepartmentCode == dept &&  x.UserId != headid && x.UserId != repid
                    select new BriefEmp
                    {
                        userId = x.UserId,
                        name = x.Name
                    };

            return q.ToList() ;
        }

    }
}
