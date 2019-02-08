using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class retrieveController : ControllerBase
    {
        private readonly SSISTeam5Context _context;
        public retrieveController(SSISTeam5Context context)
        {
            _context = context;
        }
        [HttpGet]
        public List<string> GetList(string id)
        {
            List<string> result = new List<string>();
            List<StationeryRetrievalList> source = new BizLogic().GetStationeryRetrievalLists();
            //List<retrievetemp> result = new List<retrievetemp>();
            foreach (StationeryRetrievalList temp in source)
            {
                //Catalogue cata = _context.Catalogue.Where(s => s.ItemName == temp.ItemName).ToList().First();
                //retrievetemp tmp = new retrievetemp();
                //tmp.Catagory = cata.Category;
                //tmp.ItemName = cata.ItemName;
                //tmp.Count = temp.Quantity;
                //if (temp.QuantityRetrieved == 0)
                //    tmp.stats = false;
                //else
                //    tmp.stats = true;
                result.Add(temp.ItemNumber);
            }
            return result;
        }
        [HttpGet("info/{id}")]
        public List<string> GetVoucher(string id)
        {
            List<string> result = new List<string>();
            List<StationeryRetrievalList> source = new BizLogic().GetStationeryRetrievalLists();
            StationeryRetrievalList ad = source.Where(s => s.ItemNumber == id).ToList().First();
            if (ad == null)
            {
                return null;
            }
            else
            {
                BizLogic k = new BizLogic();
                Catalogue cata = _context.Catalogue.Where(s => s.ItemName == ad.ItemName).ToList().First();
                result.Add(cata.Category);
                result.Add(ad.ItemName);
                result.Add(ad.Quantity.ToString());
                result.Add(ad.QuantityRetrieved.ToString());
                //this part in fact is price
                if (ad.Quantity == ad.QuantityRetrieved)
                    result.Add("finished");
                else
                    result.Add("Unfinished");
                return result;
            }
        }
        [HttpGet("category/{id}")]
        public List<string> GetList1(string id)
        {
            List<string> result = new List<string>();
            List<StationeryRetrievalList> source = new BizLogic().GetStationeryRetrievalLists();
            List<Catalogue> l = _context.Catalogue.Where(s => s.Category == id).ToList();
            List<string> k = new List<string>();
            foreach (Catalogue h in l)
            {
                k.Add(h.ItemNumber);
            }
            List<StationeryRetrievalList> finalsource = source.Where(s => k.Contains(s.ItemNumber)).ToList();
            //List<retrievetemp> result = new List<retrievetemp>();
            foreach (StationeryRetrievalList temp in finalsource)
            {
                //Catalogue cata = _context.Catalogue.Where(s => s.ItemName == temp.ItemName).ToList().First();
                //retrievetemp tmp = new retrievetemp();
                //tmp.Catagory = cata.Category;
                //tmp.ItemName = cata.ItemName;
                //tmp.Count = temp.Quantity;
                //if (temp.QuantityRetrieved == 0)
                //    tmp.stats = false;
                //else
                //    tmp.stats = true;
                result.Add(temp.ItemNumber);
            }
            return result;
        }
        [HttpGet("get")]
        public List<string> GetRetrieve()
        {
            List<string> result = new List<string>();
            List <StationeryRetrievalList>  source=new BizLogic().GetStationeryRetrievalLists();
            //List<retrievetemp> result = new List<retrievetemp>();
            foreach(StationeryRetrievalList temp in source)
            {
                Catalogue cata = _context.Catalogue.Where(s => s.ItemName == temp.ItemName).ToList().First();
                //retrievetemp tmp = new retrievetemp();
                //tmp.Catagory = cata.Category;
                //tmp.ItemName = cata.ItemName;
                //tmp.Count = temp.Quantity;
                //if (temp.QuantityRetrieved == 0)
                //    tmp.stats = false;
                //else
                //    tmp.stats = true;
                result.Add(cata.ItemNumber);
            }
            return result;
        }
        [HttpGet("get/{id}/{mode}")]
        public List<string> GetRetrieveDetail(string id,string mode)
        {
            List<string> result = new List<string>();
            List<StationeryRetrievalList> source = new BizLogic().GetStationeryRetrievalLists();
            StationeryRetrievalList temp = source.Where(s => s.ItemNumber == id).ToList().First();
            Catalogue cata = _context.Catalogue.Where(s => s.ItemNumber == id).ToList().First();
            result.Add(cata.Category);
            result.Add(cata.ItemName);
            if (mode.Equals("summary"))
                result.Add(cata.Out.ToString());
            else
            {
                result.Add(temp.Quantity.ToString());
            }
            if (temp.QuantityRetrieved == temp.Quantity)
                result.Add("finished");
            else
                result.Add("waiting");
            return result;
        }
        [HttpGet("PF/{amount}/{rdid}")]
        public async Task<string> PFitemAsync(string amount, string rdid)
        {
            List<StationeryRetrievalList> source = new BizLogic().GetStationeryRetrievalLists();
            StationeryRetrievalList temp = source.Where(s => s.ItemName.Contains(rdid)).ToList().First();
            //this part input change out
            //temp.QuantityDelivered = int.Parse(amount);
            //_context.Update(temp);
            //await _context.SaveChangesAsync();
            new BizLogic().UpdateCatalogueOutAndStockAfterRetrieval(temp.ItemNumber,int.Parse(amount));
            return "123";
        }
    }
    public class retrievetemp
    {
        public string Catagory;
        public string ItemName;
        public int Count;
        public bool stats;
    }
}