using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class recorddetailController : ControllerBase
    {
        private readonly SSISTeam5Context _context;
        public recorddetailController(SSISTeam5Context context)
        {
            _context = context;
        }
        [HttpGet("{rrid}")]
        public List<Dictionary<string,string>> GetDetail(string rrid)
        {
            List<Dictionary<string, string>> result = null;
            List<RecordDetails> recordDetails = _context.RecordDetails.Where(s => s.Rrid == rrid).ToList();
            if (recordDetails == null)
            {
                return (null);
            }
            foreach(RecordDetails emp in recordDetails)
            {
                Dictionary<string, string> temp = null;
                temp.Add("RDID",emp.Rdid.ToString());
                temp.Add("RRID",emp.Rrid);
                temp.Add("ItemName",emp.ItemNumberNavigation.ItemName);
                temp.Add("Quantity",emp.Quantity.ToString());
                temp.Add("Remark",emp.Remark);
                result.Add(temp);
            }
            return result;
        }
    }
}