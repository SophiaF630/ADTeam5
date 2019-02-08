using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using ADTeam5.BusinessLogic;
using ADTeam5.ViewModels;
using System.Data.Entity;

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    //[ApiController]
    public class voucherController : Controller
    {
        private readonly SSISTeam5Context _context;
        public voucherController(SSISTeam5Context context)
        {
            _context = context;
        }
        // GET: api/voucher
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/voucher/5
        [HttpGet("{id}")]
        public List<string> GetList(string id)
        {
            List<string> result = new List<string>();
            //int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<AdjustmentRecord> dis = _context.AdjustmentRecord.Where(s => s.ClerkId.ToString() == id).ToList();
            if (dis == null)
                return null;
            foreach (AdjustmentRecord i in dis)
            {
                if (i.VoucherNo != ("VTemp"+id))
                {
                    result.Add(i.VoucherNo);
                }
            }
            return result;
        }
        [HttpGet("info/{id}")]
        public List<string> GetVoucher(string id)
        {
            List<string> result = new List<string>();
            AdjustmentRecord ad = _context.AdjustmentRecord.Where(s => s.VoucherNo == id).ToList().First();
            if(ad == null)
            {
                return null;
            }
            else
            {
                BizLogic k = new BizLogic();
                result.Add(ad.VoucherNo);
                result.Add(ad.IssueDate.Date.ToString());
                result.Add(k.getPriceForAdjust(ad.VoucherNo));
                //this part in fact is price
                result.Add(ad.Status);
                return result;
            }
        }
        //here start issue voucher
        [HttpGet("manager/{id}")]
        public async Task<List<string>> GetList1Async(string id)
        {
            List<string> result = new List<string>();
            //int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<AdjustmentRecord> dis = _context.AdjustmentRecord.Where(s => s.Status== "Pending Approval").ToList();
            if (dis == null)
                return null;
            string role = null;
            User user = _context.User.Where(s => s.UserId == int.Parse(id)).First();
            Department department = _context.Department.Where(s => s.DepartmentCode == user.DepartmentCode).First();
            if (department.DepartmentCode == "STAS")//this part should add the store code
            {
                if (department.HeadId == user.UserId)
                {
                    role = "Manager";
                }
                else if (department.RepId == user.UserId)
                {
                    role = "Superviser";
                }
            }
            foreach (AdjustmentRecord i in dis)
            {
                if (role == "Manager"&& double.Parse(new BizLogic().getPriceForAdjust(i.VoucherNo))>= 250)
                {
                    
                    result.Add(i.VoucherNo);
                }
                else if(role == "Superviser" && double.Parse(new BizLogic().getPriceForAdjust(i.VoucherNo)) <= 250)
                {
                    result.Add(i.VoucherNo);
                }
            }
            return result;
        }
        [HttpGet("manager/info/{id}")]
        public List<string> GetVoucher1(string id)
        {
            List<string> result = new List<string>();
            AdjustmentRecord ad = _context.AdjustmentRecord.Where(s => s.VoucherNo == id).ToList().First();
            if (ad == null)
            {
                return null;
            }
            else
            {
                BizLogic k = new BizLogic();
                result.Add(ad.VoucherNo);
                var l = _context.User.Where(s => s.UserId == ad.ClerkId).ToList().First();
                result.Add(l.Name);
                result.Add(k.getPriceForAdjust(ad.VoucherNo));
                //this part in fact is price
                result.Add(ad.Status);
                return result;
            }
        }

        //here start voucher details
        [HttpGet("details/{id}")]
        public List<string> GetdetailList(string id)
        {
            AdjustmentRecord ad = _context.AdjustmentRecord.Where(s => s.VoucherNo == id).ToList().First();
            if (id.Contains("VTemp") && ad == null)
            {
                var adjustmentRecord = new AdjustmentRecord()
                {
                    VoucherNo = id,
                    IssueDate = DateTime.Today,
                    ClerkId = int.Parse(id.Replace("VTemp", "")),
                    Status = "draft"
                };
                _context.AdjustmentRecord.Add(adjustmentRecord);
                _context.SaveChanges();
            }
            List<string> result = new List<string>();
            //int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<RecordDetails> dis = _context.RecordDetails.Where(s => s.Rrid == id).ToList();
            if (dis == null)
                return null;
            foreach (RecordDetails i in dis)
            {
                result.Add(i.Rdid.ToString());
            }
            return result;
        }
        [HttpGet("detailsinfo/{id}")]
        public List<string> GetVoucherDetails(string id)
        {
            List<string> result = new List<string>();
            RecordDetails ad = _context.RecordDetails.Where(s => s.Rdid.ToString() == id).ToList().First();
            if (ad == null)
            {
                return null;
            }
            else
            {
                var tem = _context.Catalogue.Where(s => s.ItemNumber == ad.ItemNumber).ToList().First();
                result.Add(tem.ItemName);
                result.Add(ad.Rdid.ToString());
                //this part in fact is price
                result.Add(ad.Quantity.ToString());
                result.Add(ad.Remark);
                return result;
            }
        }
        [HttpGet("Cata/List")]
        public List<string> getCataList()
        {
            List<string> result = new List<string>();
            List<Catalogue> List = _context.Catalogue.ToList();
            foreach(Catalogue item in List)
            {
                if (!result.Contains(item.Category))
                    result.Add(item.Category);
            }
            return result;
        }
        [HttpGet("Cata/List/{id}")]
        public List<string> getItemList(string id)
        {
            List<string> result = new List<string>();
            List<Catalogue> List = _context.Catalogue.Where(s => s.Category == id).ToList();
            foreach (Catalogue item in List)
            {
                if (!result.Contains(item.ItemName))
                    result.Add(item.ItemName);
            }
            return result;
        }
        //apply voucher
        [HttpGet("ApplyVoucher/{id}")]
        public void ApplyVoucher(int id)
        {
            List<RecordDetails> source = _context.RecordDetails.Where(s => s.Rrid == ("VTemp" + id)).ToList();
            string vono = new BizLogic().IDGenerator("V");
            //shengcheng vocher
            AdjustmentRecord temp = new AdjustmentRecord();
            temp.VoucherNo = vono;
            temp.ClerkId = id;
            temp.IssueDate = DateTime.Now;
            temp.Status = "Pending Approval";
            _context.AdjustmentRecord.Add(temp);
            foreach(RecordDetails i in source)
            {
                i.Rrid = vono;
                _context.RecordDetails.Update(i);
            }
            _context.SaveChanges();
            //save changes
        }
        //issuevoucher
        [HttpGet("IssueVoucher/{id}/{uid}")]
        public void IssueVoucher(string id,string uid)
        {
            var ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
            if (ar != null)
            {
                var arList = _context.RecordDetails.Where(x => x.Rrid == id);
                if (arList != null)
                {
                    //value of voucher
                    decimal? amount = new BizLogic().GetTotalAmountForVoucher(id);
                    decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
                    decimal? totalAmount = amount + GST;
                            ar.Status = "Approved";
                            ar.SuperviserId = int.Parse(uid);
                            ar.ApproveDate = DateTime.Now.Date;
                            _context.AdjustmentRecord.Update(ar);
                            _context.SaveChanges();

                            foreach (var item in arList.ToList())
                            {
                                new BizLogic().UpdateCatalogueStockAfterSuppDeliveryOrVoucherApproved(item.ItemNumber, item.Quantity);
                                int balance = _context.Catalogue.Find(item.ItemNumber).Stock;
                                new BizLogic().UpdateInventoryTransRecord(item.ItemNumber, id, item.Quantity, balance);
                            }
                    }
                }
            }

        //reject voucher
        [HttpGet("RejectIssueVoucher/{id}/{uid}")]
        public void RejectIssueVoucher(string id,string uid)
        {
            var ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
            if (ar != null)
            {
                    ar.SuperviserId = int.Parse(uid);
                    ar.Status = "Draft";
                    ar.ApproveDate = DateTime.Now.Date;
                    _context.AdjustmentRecord.Update(ar);
                    _context.SaveChanges();
            }
        }
        //save voucher
        [HttpPost("save")]
        public async Task<string> saveVoucherAsync(details source)
        {
            RecordDetails k = new RecordDetails();
            var h = _context.Catalogue.Where(s => s.ItemName == source.ItemName).First().ItemNumber;
            k.ItemNumber = h;
            k.Rrid = source.VoNo;
            k.Remark = source.Remark;
            k.Quantity = source.Quantity;
            _context.RecordDetails.Add(k);
            await _context.SaveChangesAsync();
            return "ok";
        }
        // POST: api/voucher
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/voucher/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
