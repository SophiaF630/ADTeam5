using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using ADTeam5.BusinessLogic;
using ADTeam5.ViewModels;

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
        public List<string> GetList1(string id)
        {
            List<string> result = new List<string>();
            //int idID = _context.CollectionPoint.Where(s => s.CollectionPointName.Contains(id)).ToList().First().CollectionPointId;
            List<AdjustmentRecord> dis = _context.AdjustmentRecord.Where(s => s.Status== "Submitted").ToList();
            if (dis == null)
                return null;
            foreach (AdjustmentRecord i in dis)
            {
                if (i.VoucherNo != ("VTemp" + id))
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
