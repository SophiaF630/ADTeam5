using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.DeptAPIController
{
    [Route("api/[controller]")]
    public class DeptAPIController : Controller
    {
        private readonly SSISTeam5Context context;
        AndroidDeptBizLogic b = new AndroidDeptBizLogic();

        // GET: api/<controller>/ENGL
        [HttpGet("{dept}")]
        public IEnumerable<string> Get(string dept)
        {
            return b.ListEmployeeNames(dept);
        }

        // GET api/<controller>/GetRep/ENGL
        [HttpGet("GetRep/{dept}")]
        public BriefEmp GetRep(string dept)
        {
            return b.GetRep(dept);
        }

        // GET api/<controller>/GetRep/ENGL
        [HttpGet("GetBriefEmp/{dept}")]
        public IEnumerable<BriefEmp> GetBriefEmp(string dept)
        {
            return b.ListEmployeesBrief(dept);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
