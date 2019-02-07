using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class orderdetailController : ControllerBase
    {
        private readonly SSISTeam5Context _context;
        public orderdetailController(SSISTeam5Context context)
        {
            _context = context;
        }
        [HttpGet("{rrid}")]
        public async Task<List<string>> GetDetail(string rrid)
        {
            List<string> result = new List<string>();
            List<RecordDetails> details = _context.RecordDetails.Where(s => s.Rrid == rrid).ToList();
            //this part should add some logic judge the departments that has order
            if (details == null)
            {
                return result;
            }
            else
            {
                //RecordDetails temp = details.First();
                List<DisbursementList> emps = _context.DisbursementList.Where(s => s.Dlid == rrid).ToList();
                DisbursementList emp = emps.First();
                List<Department> emp2 = _context.Department.Where(s => s.DepartmentCode == emp.DepartmentCode).ToList();
                Department emp3 = emp2.First();
                result.Add(emp3.DepartmentName);
                string name = _context.User.Where(s => s.UserId == emp3.RepId).ToList().First().Name;
                result.Add(name);
                //result.Add(emp3.Rep.Name);
                string pointName = _context.CollectionPoint.Where(s => s.CollectionPointId == emp3.CollectionPointId).ToList().First().CollectionPointName;
                result.Add(pointName);
                //result.Add(emp3.CollectionPoint.CollectionPointName);
                String orderDetails = "";
                String remarkDetails = "";
                foreach (RecordDetails temp in details)
                {
                    orderDetails += temp.Quantity;
                    List<Catalogue> i = _context.Catalogue.Where(s => s.ItemNumber == temp.ItemNumber).ToList();
                    Catalogue k = i.First();
                    orderDetails += k.Category;
                    orderDetails += "(";
                    orderDetails += k.ItemName;
                    orderDetails += ")";
                    if (temp.QuantityDelivered != 0)
                    {
                        orderDetails += temp.QuantityDelivered;
                        orderDetails += "delivered";
                    }
                    orderDetails += "\n";
                    if (temp.Remark != null)
                    {
                        remarkDetails += k.Category;
                        remarkDetails += ":";
                        remarkDetails += temp.Remark;
                        remarkDetails += "\n";
                    }
                }
                result.Add(orderDetails);

                result.Add(remarkDetails);
                //var depart = await _context.Department
                //.Include(d => d.DepartmentCodeNavigation)
                //.FirstOrDefaultAsync(m => m.UserId == id);
                return result;
            }
        }
    }
}