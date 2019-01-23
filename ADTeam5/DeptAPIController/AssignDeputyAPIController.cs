using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.DeptAPIController
{
    [Route("api/[controller]")]
    public class AssignDeputyAPIController : Controller
    {
        DeptBizLogic b = new DeptBizLogic();
        static int currentDeputyId;

        // GET: api/<controller>
        [HttpGet]
        public bool CheckDeputy()
        {
            bool coveringHeadExists;
            //Filter accoriding to dept of person using this
            Models.Department d1 = b.getDepartmentDetails("ENGL");
            if(d1.CoveringHeadId == null)
            {
                coveringHeadExists = false;
            }
            else
            {
                coveringHeadExists = true;
            }
            return coveringHeadExists;
        }

        // GET api/<controller>/CurrentDeputyHead/ENGL
        [HttpGet("CurrentDeputyHead/{dept}")]
        public DepartmentCoveringHeadRecord GetCurrentDeputy(string dept)
        {
            Department department = b.getDepartmentDetails(dept);
            if(department.CoveringHeadId != null)
            {
                currentDeputyId = (int) department.CoveringHeadId;
                DepartmentCoveringHeadRecord d = b.findCurrentDeputyHeadToEdit(currentDeputyId);
                return d;
            }
            else
            {
                return null;
            }
        }

        // GET api/<controller>/CurrentDeputyHead/ENGL
        [HttpGet("EmployeeNames/{dept}")]
        public List<String> EmployeeNames(string dept)
        {
            int headid = b.getDeptHeadID(dept);
            int repid = b.getRepId(dept);
            List<String> list= b.populateAssignDeputy(dept, repid, headid);
            return list;
        }

        // POST api/<controller>
        [HttpPut("CurrentDeputyHead/Save/{dept}")]
        public void Post([FromBody]string dept, string name, DateTime newStartDate, DateTime newEndDate)
        {
            Department d = b.getDepartmentDetails(dept);

            if(d.CoveringHeadId == null)
            {
                //New DepartmentCoveringHeadRecord
                b.saveDeputy(dept, name, newStartDate, newEndDate);

            }
            else
            {
                //Override previous DepartmentCoveringHeadRecord
                b.updateDeputy(dept, name, newStartDate, newEndDate);
            }

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
