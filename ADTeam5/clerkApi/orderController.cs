using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADTeam5.clerkApi
{
    public partial class order
    {
        public string DepName { get; set; }
        public string RRID { get; set; }
        public string DepRep { get; set; }
        public string status { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class orderController : ControllerBase
    {
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        public orderController(SSISTeam5Context context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<List<string>> GetAsync(string id)
        {
            List<string> result = new List<string>();
            int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<DisbursementList> dis = _context.DisbursementList.Where(s => s.CollectionPointId == idID && s.CompleteDate==null).ToList();
            if (dis == null)
                return null;
            List<Department> deps = new List<Department>();
            foreach(DisbursementList i in dis)
            {
                deps.Add(_context.Department.Where(s => s.DepartmentCode == i.DepartmentCode).ToList().First());
            }
                
            //this part should add some logic judge the departments that has order
            if (deps == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < deps.Count(); i++)
                {
                    List<DisbursementList> orders = _context.DisbursementList.Where(s => s.DepartmentCode == deps[i].DepartmentCode).ToList();
                    if(orders != null)
                    result.Add(deps[i].DepartmentCode);
                    //this part check if this record still work
                }
                //var depart = await _context.Department
                //.Include(d => d.DepartmentCodeNavigation)
                //.FirstOrDefaultAsync(m => m.UserId == id);
                return result;
            }
        }
        [HttpGet("orders/{departmentcode}")]
        public async Task<List<string>> GetDepartmentDetail(string departmentcode)
        {
            List<string> result = new List<string>();
            var temp = _context.DisbursementList.Where(s => s.DepartmentCode == departmentcode&& s.CompleteDate == null).ToList();
            var temp1 = temp.First();
            if (temp1 == null)
            return result;
            else
            {
                var tem = _context.Department.Where(s => s.DepartmentCode == departmentcode).ToList();
                var tem1 = tem.First();
                result.Add(tem1.DepartmentName);
                result.Add(temp1.Dlid);
                string name = _context.User.Where(s => s.UserId == tem1.RepId).ToList().First().Name;
                result.Add(name);
                result.Add(temp1.Status);
                return result;
            }
        }
        [HttpGet("pfpart/{id}")]
        public List<string> GetPfPart(string id)
        {
            List<string> result = new List<string>();
            //int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<RecordDetails> dis = _context.RecordDetails.Where(s => s.Rrid == id).ToList();
            if (dis == null)
                return null;
            List<Department> deps = new List<Department>();
            foreach(RecordDetails i in dis)
            {
                result.Add(i.Rdid.ToString());
            }
            return result;
            //this part should add some logic judge the departments that has order
        }
        [HttpGet("pfparts/{id}")]
        public List<string> GetPfParts(string id)
        {
            List<string> result = new List<string>();
            var temp = _context.RecordDetails.Where(s => s.Rdid.ToString() == id).ToList().First();
            if (temp == null)
                return result;
            else
            {
                result.Add(id);
                var tem = _context.Catalogue.Where(s => s.ItemNumber == temp.ItemNumber).ToList().First();
                result.Add(tem.ItemName);
                result.Add(temp.Quantity.ToString());
                result.Add(temp.Remark);
                return result;
            }
        }
        [HttpGet("collection")]
        public List<string> GetCollectionPoints()
        {
            List<string> result = new List<string>();
            var temp = _context.CollectionPoint.ToList();
            foreach (CollectionPoint tem in temp)
            {
                result.Add(tem.CollectionPointName);
            }
            return result;
        }
        [HttpGet("NoShow/{id}")]
        public void noShow(string id)
        {
            var disbursementList = _context.DisbursementList.Find(id);
            disbursementList.Status = "No Show";
            disbursementList.CompleteDate = DateTime.Now;
            _context.DisbursementList.Update(disbursementList);
            _context.SaveChanges();

            //update out and stock
            var itemToReturn = _context.RecordDetails.Where(x => x.Rrid == id);
            if (itemToReturn != null)
            {
                foreach (var item in itemToReturn.ToList())
                {
                    int quantityRequested = item.Quantity;
                    var q = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber);
                    if (q != null)
                    {
                        int outQty = q.Out;
                        int preStock = q.Stock;
                        q.Stock = preStock + quantityRequested;
                        q.Out = outQty - quantityRequested;

                        _context.Catalogue.Update(q);
                        _context.SaveChanges();
                    }
                }
            }

            return ;
        }
        [HttpGet("Delivered/{id}")]
        public void delivered(string id)
        {
            string depCode = _context.DisbursementList.Find(id).DepartmentCode;
            List<RecordDetails> tempDisbursementListDetails = _context.RecordDetails.Where(s => s.Rrid == id).ToList();
            string dlID = b.IDGenerator("DL");
            foreach (var item in tempDisbursementListDetails)
            {
                string itemNo = item.ItemNumber;
                int qtyDelivered = item.QuantityDelivered;
                int rdid = item.Rdid;
                string remark = item.Remark;
                if (qtyDelivered == 0)
                {
                    qtyDelivered = item.Quantity;
                }
                //generate a disbursement list if partial fulfilled
                if (qtyDelivered != item.Quantity)
                {
                    int qty = item.Quantity - qtyDelivered;
                    b.GenerateDisbursementListForPartialFulfillment(itemNo, qty, remark, depCode, dlID);
                }

                b.UpdateCatalogueOutAfterDelivery(itemNo, qtyDelivered);
                b.UpdateQuantityDeliveredAfterDelivery(qtyDelivered, rdid);

                int balance = _context.Catalogue.Find(itemNo).Stock;
                b.UpdateInventoryTransRecord(itemNo, id, -qtyDelivered, balance);
            }

            //update disbursement list status
            var disbursementList = _context.DisbursementList.Find(id);
            disbursementList.Status = "Completed";
            disbursementList.CompleteDate = DateTime.Now;
            _context.DisbursementList.Update(disbursementList);
            _context.SaveChanges();

            //TempData["CorrectPassword"] = "Successful confirmation. Delivery completed!";

            return ;
        }
        [HttpGet("Deliveredpf/{id}")]
        public void deliveredpf(string id)
        {
            string depCode = _context.DisbursementList.Find(id).DepartmentCode;
            List<RecordDetails> tempDisbursementListDetails = _context.RecordDetails.Where(s => s.Rrid == id).ToList();
            string dlID = b.IDGenerator("DL");
            foreach (var item in tempDisbursementListDetails)
            {
                string itemNo = item.ItemNumber;
                int qtyDelivered = item.QuantityDelivered;
                int rdid = item.Rdid;
                string remark = item.Remark;

                //generate a disbursement list if partial fulfilled
                if(qtyDelivered == -1)
                {
                    qtyDelivered = item.Quantity;
                }
                if (qtyDelivered != item.Quantity)
                {
                    int qty = item.Quantity - qtyDelivered;
                    b.GenerateDisbursementListForPartialFulfillment(itemNo, qty, remark, depCode, dlID);
                }

                b.UpdateCatalogueOutAfterDelivery(itemNo, qtyDelivered);
                b.UpdateQuantityDeliveredAfterDelivery(qtyDelivered, rdid);

                int balance = _context.Catalogue.Find(itemNo).Stock;
                b.UpdateInventoryTransRecord(itemNo, id, -qtyDelivered, balance);
            }

            //update disbursement list status
            var disbursementList = _context.DisbursementList.Find(id);
            disbursementList.Status = "Completed";
            disbursementList.CompleteDate = DateTime.Now;
            _context.DisbursementList.Update(disbursementList);
            _context.SaveChanges();

            //TempData["CorrectPassword"] = "Successful confirmation. Delivery completed!";

            return;
        }
    }
}