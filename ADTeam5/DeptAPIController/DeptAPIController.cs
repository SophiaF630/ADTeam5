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
    [Route("[controller]")]
    public class DeptAPIController : Controller
    {
        private readonly SSISTeam5Context context = new SSISTeam5Context();
        AndroidDeptBizLogic b = new AndroidDeptBizLogic();

        // GET: api/<controller>/ENGL
        [HttpGet("{dept}")]
        public List<BriefEmp> Get(string dept)
        {
            return b.ListEmployees(dept);
        }

        // GET api/<controller>/GetRep/ENGL
        [HttpGet("GetRep/{dept}")]
        public BriefEmp GetRep(string dept)
        {
            return b.GetRep(dept);
        }

        // POST api/<controller>
        [HttpPost("SaveBriefEmp")]
        public void SaveBriefEmp(BriefDept briefDept)
        {
            b.SaveNewDeptRep(briefDept);
        }

        [HttpGet("OutstandingOrders/{dept}")]
        public List<BriefEmpReq> GetOutstandingOrders(string dept)
        {
            return b.getOutstandingOrders(dept);
        }

        [HttpGet("RecordDetails/{rrid}")]
        public List<BriefRecDetails> GetRecordDetails(string rrid)
        {
            return b.getRecordDetails(rrid);
        }

        [HttpPost]
        [Route("ApproveRequest")]
        public void ApproveRequest(BriefEmpReq briefEmpReq)
        {
            b.ApproveRequest(briefEmpReq);
        }

        [HttpPost("RejectRequest")]
        public void RejectRequest(BriefEmpReq briefEmpReq)
        {
            b.RejectRequest(briefEmpReq);
        }

        [HttpGet("CollectionPassword/{dept}")]
        public BriefDept CollectionPassword(string dept)
        {
            return b.generateCollectionPassword(dept);
        }

        [HttpGet("CurrentCollectionPoint/{dept}")]
        public BriefDept2 CurrentCollectionPoint(string dept)
        {
            return b.currentCollectionPoint(dept);
        }

        [HttpPost("ChangeCollectionPoint")]
        public void ChangeCollectionPoint(BriefDept2 briefDept2)
        {
            b.changeCollectionPoint(briefDept2);
        }

        [HttpGet("AllCollectionPoints")]
        public List<BriefDept2> AllCollectionPoints()
        {
            return b.listCollectionPoints();
        }

        [HttpGet("GetDisbursement/{dept}")]
        public BriefDisbursement GetDisbursement(string dept)
        {
            return b.getBriefDisbursement(dept);
        }
    }
}
