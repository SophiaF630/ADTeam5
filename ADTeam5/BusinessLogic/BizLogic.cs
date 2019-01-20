using ADTeam5.Models;
using ADTeam5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.BusinessLogic
{
    public class BizLogic
    {
        private readonly SSISTeam5Context _context = new SSISTeam5Context();

        //public BizLogic(SSISTeam5Context context)
        //{
        //    _context = context;
        //}

        //public BizLogic()
        //{
        //}

        //Find StationeryRetrival startDate
        private DateTime StationeryRetrivalStartDate()
        {
            DateTime start = DateTime.Today.Date;
            if (start.DayOfWeek >= DayOfWeek.Thursday)
            {
                start = start.AddDays(-7);
            }
            while (start.DayOfWeek != DayOfWeek.Thursday)
            {
                start = start.AddDays(-1);
            }
            return start;
        }

        //Find StationeryRetrival cutoffDate
        private DateTime StationeryRetrivalCutoffDate()
        {
            DateTime cutoff = DateTime.Today.Date;
            if (cutoff.DayOfWeek >= DayOfWeek.Wednesday)
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                    cutoff = cutoff.AddDays(-1);
            }
            else
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                    cutoff = cutoff.AddDays(1);
            }
            return cutoff;
        }

        //Find NextStationeryRetrivalStartDate
        private DateTime NextStationeryRetrivalStartDate()
        {
            DateTime start = DateTime.Now;
            do
            {
                start = start.AddDays(1);
            } while (start.DayOfWeek != DayOfWeek.Thursday);
            return start;
        }

        /*Generate RecoredID for EmployeeRequestRecord(depcode), PurchaseOrderRecord("PO"),
        AdjustmentRecord("V"), DisbursementList("DL") (e.g. XX201901150001)*/
        public string IDGenerator(string prefix)
        {
            DateTime today = DateTime.Today.Date;
            List<RecordDetails> rdList = _context.RecordDetails.ToList();

            string ID_string = today.Year.ToString("0000")
                                   + today.Month.ToString("00")
                                   + today.Day.ToString("00")
                                   + "0001";
            long ID = Convert.ToInt64(ID_string);
            string result = prefix + ID_string;
            List<string> rdidList = new List<string>();
            foreach (RecordDetails rd in rdList)
            {                
                rdidList.Add(rd.Rrid);                
            }
            while (rdidList.Contains(result))
            {
                ID++;
                result = prefix + ID.ToString();
            }
            return result;
        }


        //Generate Disbursement List for a department
        public List<RecordDetails> GenerateDisbursementListDetails(string depCode)
        {
            DateTime start = StationeryRetrivalStartDate();
            DateTime cutoff = StationeryRetrivalCutoffDate();

            //DisbursementListDetails for a department
            List<RecordDetails> result = new List<RecordDetails>();

            //Check if a pending delivery disbursement list exists
            List<DisbursementList> dlList = _context.DisbursementList
                .Where(x => x.DepartmentCode == depCode && x.Status == "Pending Delivery").ToList();    
            
            //Get EmployeeRequestRecord of a department, check if it's null
            List<EmployeeRequestRecord> errList = _context.EmployeeRequestRecord
                .Where(x => x.CompleteDate >= start && x.CompleteDate <= cutoff && x.DepCode == depCode && x.Status == "Approved")
                .ToList();

            //Get all RecordDetails
            List<RecordDetails> rdList = _context.RecordDetails.ToList();

            // Check if the disbursement list exists in the database
            var dl = _context.DisbursementList.FirstOrDefault(x => x.DepartmentCode == depCode && x.Status == "Pending Delivery");
            if (errList != null)
            {
                if (dl == null)
                {
                    //create a new dl  
                    dl = new DisbursementList()
                    {
                        Dlid = IDGenerator("DL"),
                        StartDate = DateTime.Now,
                        DepartmentCode = depCode,
                        EstDeliverDate = null,
                        CompleteDate = null,
                        RepId = _context.Department.Find(depCode).RepId,
                        CollectionPointId = _context.Department.Find(depCode).CollectionPointId,
                        Status = "Pending Delivery"
                    };
                    Console.WriteLine(dl.StartDate);
                    _context.DisbursementList.Add(dl);
                    _context.SaveChanges();
                }

                //Find all needed EmployeeRequestRecordID, add to a list
                List<string> rridList = new List<string>();
                foreach (EmployeeRequestRecord err in errList)
                {
                    rridList.Add(err.Rrid);
                }

                //Select out needed EmployeeRequestRecord details
                List<RecordDetails> selectederrList = new List<RecordDetails>();
                foreach (RecordDetails r in rdList)
                {
                    if (rridList.Contains(r.Rrid))
                    {
                        selectederrList.Add(r);
                    }
                }

                foreach (RecordDetails r in selectederrList)
                {
                    //check if dl record exists
                    var rd = _context.RecordDetails.FirstOrDefault(x => x.Rrid == dl.Dlid && x.ItemNumber == r.ItemNumber && x.Remark == null);
                    rd.Quantity = 0;
                    if (rd == null)
                    {
                        rd = new RecordDetails() { Rrid = dl.Dlid, ItemNumber = r.ItemNumber, Quantity = r.Quantity };
                        _context.RecordDetails.Add(rd);
                    }
                    else
                    {
                        rd.Quantity += r.Quantity;
                    }
                    _context.SaveChanges();
                }
                
                result = _context.RecordDetails.Where(x => x.Rrid == dl.Dlid).ToList();
            }
            return result;            
        }

        //Generate Stationery Retrieval List for a department
        public List<StationeryRetrievalList> GetStationeryRetrievalLists()
        {
            List<StationeryRetrievalList> result = new List<StationeryRetrievalList>();

            //get pending delivery disbursement list
            List<DisbursementList> dlList = _context.DisbursementList.Where(x => x.Status == "Pending Delivery").ToList();
           
            //Find all needed Disbursement List ID, add to a list
            List<string> dlidList= new List<string>();
            foreach (DisbursementList dl in dlList)
            {
                dlidList.Add(dl.Dlid);
            }

            //Get all RecordDetails
            List<RecordDetails> rdList = _context.RecordDetails.ToList();

            //Select out needed Disbursement List Record details
            List<RecordDetails> selecteddlList = new List<RecordDetails>();
            foreach (RecordDetails r in rdList)
            {
                if (dlidList.Contains(r.Rrid))
                {
                    selecteddlList.Add(r);
                }
            }


            var q = from x in selecteddlList
                     group x by x.ItemNumber into g
                     select new { g.Key, Quantiy = g.Sum(y => y.Quantity) };

            foreach (var i in q.ToList())
            {
                StationeryRetrievalList srl = new StationeryRetrievalList();
                srl.ItemNumber = i.Key;
                srl.ItemName = _context.Catalogue.Find(i.Key).ItemName;
                srl.Quantity = i.Quantiy;

                result.Add(srl);
            }

            return result;
        }
        public Models.Department getDepartmentDetails(string dept)
        {
            var q1 = _context.Department.Where(x => x.DepartmentCode == dept).First();
            Models.Department d = q1;
            return d;
        }

        public Models.User getUser(int userid)
        {
            var q = _context.User.Where(x => x.UserId == userid).First();
            Models.User u= q;
            return u;
        }


    }
}
