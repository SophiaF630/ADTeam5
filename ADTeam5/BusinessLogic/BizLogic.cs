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
            DateTime start = DateTime.Now;
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
            DateTime cutoff = DateTime.Now;
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
        public List<DisbursementList> GenerateDisbursementLists(string depCode)
        {
            DateTime start = StationeryRetrivalStartDate();
            DateTime cutoff = StationeryRetrivalCutoffDate();

            //Check if a pending delivery disbursement list exists
            List<DisbursementList> dlList = _context.DisbursementList
                .Where(x => x.DepartmentCode == depCode && x.Status == "Pending Delivery").ToList();    
            
            //Get EmployeeRequestRecord of a department, check if it's null
            List<EmployeeRequestRecord> errList = _context.EmployeeRequestRecord
                .Where(x => x.CompleteDate >= start && x.CompleteDate <= cutoff && x.DepCode == depCode && x.Status == "Approved")
                .ToList();

            DisbursementList dl = new DisbursementList();
            if (dlList == null && errList != null)
            {
                //create a new dl
                
                dl.Dlid = IDGenerator("DL");
            }
            else if (dlList != null && errList != null)
            {
                //update dl
            }

            //Add EmployeeRequestRecordID to a list
            List<string> rridList = new List<string>();
            foreach (EmployeeRequestRecord err in errList)
            {
                rridList.Add(err.Rrid);
            }

            //Get all RecordDetails
            List<RecordDetails> rdList = _context.RecordDetails.ToList();
            //Add selected details
            foreach (RecordDetails rd in rdList)
            {
                if (rridList.Contains(rd.Rrid))
                {
                    rdList.Add(rd);
                }
             }
            //Group and sum details by itemNumber
            var q = from x in rdList
                    group x by x.ItemNumber into g
                    select new { ItemNumber = g.Key, QuantityNeeded = g.Sum(y => y.Quantity) };


            var p = from x in q
                    join y in _context.Catalogue on x.ItemNumber equals y.ItemNumber
                    select new { x.ItemNumber, y.ItemName, x.QuantityNeeded };

            return dlList;            
        }

        public List<StationeryRetrievalList> GetStationeryRetrievalLists()
        {
            List<StationeryRetrievalList> result = new List<StationeryRetrievalList>();
            //foreach (var item in p)
            //{
            //    StationeryRetrievalList srList = new StationeryRetrievalList();
            //    srList.ItemNumber = item.ItemNumber;
            //    srList.ItemName = item.ItemName;
            //    srList.Quantity = item.QuantityNeeded;

            //    result.Add(srList);
            //}
            return result;
        }
    }
}
