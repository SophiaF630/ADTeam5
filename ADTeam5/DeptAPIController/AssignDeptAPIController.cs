using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.DeptAPIController
{
    [Route("api/[controller]")]
    public class AssignDeptAPIController : Controller
    {
        DeptBizLogic b = new DeptBizLogic();
        static int currentRepId;

        // GET: api/<controller>
        [HttpGet]
        public Department Get()
        {
            ////Filter according to the dept of the person using this
            //Department d = b.getDepartmentDetails("ENGL");
            //currentRepId = d.RepId;
            //User u = b.getUser(currentRepId);
            //string currentRepName = u.Name;
            //return currentRepName;
            Department d = b.getDepartmentDetails("ENGL");
            return d;
        }

        // GET api/<controller>/EmployeeNames
        [HttpGet("{EmployeeNames}")]
        public List<string> GetEmployeeNames()
        {
            int headid = b.getDeptHeadID("ENGL");
            int coveringheadid = b.getDeputyDeptHeadID("ENGL");
            List<String> EmployeeNamesList = b.getEmployeeNamesForAssignDept("ENGL", currentRepId, headid, coveringheadid);
            return EmployeeNamesList;
      
        }

        [HttpPost("{EmployeeNames}")]
        public List<string> SetEmployeeNames()
        {
            int headid = b.getDeptHeadID("ENGL");
            int coveringheadid = b.getDeputyDeptHeadID("ENGL");
            List<String> EmployeeNamesList = b.getEmployeeNamesForAssignDept("ENGL", currentRepId, headid, coveringheadid);
            return EmployeeNamesList;

        }

        // POST api/<controller>
        [HttpPost("{EmployeeNames}")]
        public int Post([FromBody]string name)
        {
            return b.updateDepartment(name, "ENGL");
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
