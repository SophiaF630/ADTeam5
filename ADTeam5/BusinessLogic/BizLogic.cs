﻿using ADTeam5.Models;
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

        //Find EstimateDeliverDate
        private DateTime EstimateDeliverDate()
        {
            DateTime estDeliverDate = StationeryRetrivalCutoffDate();
            do
            {
                estDeliverDate = estDeliverDate.AddDays(1);
            } while (estDeliverDate.DayOfWeek != DayOfWeek.Monday);
            return estDeliverDate;
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

        //Auto increment int ID generator
        public int AutoIncrementIDGenerator(List<DisbursementListDetails> disbursementListDetails)
        {
            int result = 0;
            List<int> rowIDList = new List<int>();
            foreach (DisbursementListDetails dl in disbursementListDetails)
            {
                rowIDList.Add(dl.RowID);
            }
            while (rowIDList.Contains(result))
            {
                result++;
            }
            return result;
        }


        //Generate Disbursement List for a department
        public List<RecordDetails> GenerateRecordDetailsOfDisbursementList(string depCode)
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
            if (errList.Count != 0)
            {
                if (dl == null)
                {
                    //create a new dl  
                    //DateTime estDeliverDate = EstimateDeliverDate();
                    dl = new DisbursementList()
                    {
                        Dlid = IDGenerator("DL"),
                        StartDate = DateTime.Now,
                        DepartmentCode = depCode,
                        EstDeliverDate = EstimateDeliverDate(),
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

                //groupby item
                var groupByItem = selectederrList.GroupBy(x => new { x.ItemNumber, x.Remark, x.Rrid }).
                    Select(x => new { x.Key.Rrid, x.Key.ItemNumber, x.Key.Remark, Quantity = x.Sum(y => y.Quantity) });
                foreach(var item in groupByItem)
                {
                    var q = _context.RecordDetails.FirstOrDefault(x => x.Rrid == dl.Dlid && x.ItemNumber == item.ItemNumber && x.Remark == null);
                    if (q == null)
                    {
                        RecordDetails rd = new RecordDetails() { Rrid = dl.Dlid, ItemNumber = item.ItemNumber, Quantity = item.Quantity };
                        _context.RecordDetails.Add(rd);
                    }
                    else
                    {
                        q.Quantity = item.Quantity;
                    }

                    _context.SaveChanges();
                }

                //foreach (RecordDetails r in selectederrList)
                //{
                //    //check if dl record exists
                //    //RecordDetails rd = new RecordDetails();
                //    var q = _context.RecordDetails.FirstOrDefault(x => x.Rrid == dl.Dlid && x.ItemNumber == r.ItemNumber && x.Remark == null);
                //    if (q == null)
                //    {
                //        RecordDetails rd = new RecordDetails() { Rrid = dl.Dlid, ItemNumber = r.ItemNumber, Quantity = r.Quantity };
                //        _context.RecordDetails.Add(rd);
                //    }
                //    else
                //    {
                //        RecordDetails rd = new RecordDetails();
                //        q.Quantity += r.Quantity;
                //    }

                //    _context.SaveChanges();
                //}

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
            List<string> dlidList = new List<string>();
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
                srl.QuantityRetrieved = _context.Catalogue.Find(i.Key).Out;


                result.Add(srl);
            }

            return result;
        }


        //Update stationeery out quantity and stock after retrieval
        public void UpdateCatalogueOutAndStockAfterRetrieval(string itemNumber, int outQty)
        {
            Catalogue catalogue = new Catalogue();
            var i = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber);
            if (i != null)
            {
                int preStock = i.Stock + i.Out;
                int preOut = i.Out;
                if (preStock >= outQty)
                {
                    i.Out = outQty;
                    i.Stock = preStock - outQty;
                    _context.SaveChanges();
                }
            }
        }

        //Update stationery out after delivery
        public void UpdateCatalogueOutAfterDelivery(string itemNumber, int qtyDelivered)
        {
            Catalogue catalogue = new Catalogue();
            var i = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber);
            if (i != null)
            {
                int preOut = i.Out;
                if (preOut >= qtyDelivered)
                {
                    i.Out = preOut - qtyDelivered;
                    _context.SaveChanges();
                }
            }
        }

        //Update quantity delivered out after delivery
        public void UpdateQuantityDeliveredAfterDelivery(int qtyDelivered, int rdid)
        {
            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            rd.QuantityDelivered = qtyDelivered;
            _context.SaveChanges();
        }

        //Generate a disbursement list if partial fulfilled
        public void GenerateDisbursementListForPartialFulfillment(string itemNumber, int qty, string remark, string depCode, string dlID)
        {
            //check if dl record exists
            var q = _context.DisbursementList.FirstOrDefault(x => x.Dlid == dlID);
            if (q == null)
            {
                DisbursementList dl = new DisbursementList();
                dl.Dlid = dlID;
                dl.StartDate = DateTime.Now;
                dl.EstDeliverDate = EstimateDeliverDate();
                dl.DepartmentCode = depCode;
                dl.RepId = _context.Department.Find(depCode).RepId;
                dl.CollectionPointId = _context.Department.Find(depCode).CollectionPointId;
                dl.Status = "Pending Delivery";

                _context.DisbursementList.Add(dl);
                _context.SaveChanges();
            }

            RecordDetails rd = new RecordDetails();
            rd.Rrid = dlID;
            rd.ItemNumber = itemNumber;
            rd.Quantity = qty;
            rd.QuantityDelivered = 0;
            rd.Remark = remark;
            _context.RecordDetails.Add(rd);
            _context.SaveChanges();

        }

        //Update inventory transaction record
        public void UpdateInventoryTransRecord(string itemNumber, string recordID, int qty, int balance)
        {
            if(qty != 0)
            {
                InventoryTransRecord inventoryTransRecord = new InventoryTransRecord();
                inventoryTransRecord.Date = DateTime.Now.Date;
                inventoryTransRecord.ItemNumber = itemNumber;
                inventoryTransRecord.RecordId = recordID;
                inventoryTransRecord.Qty = qty;
                inventoryTransRecord.Balance = balance;
                _context.InventoryTransRecord.Add(inventoryTransRecord);
                _context.SaveChanges();
            }           
        }

        //Change Deliver Date
        public void ChangeEstDeliverDate(string departmentName, DateTime estDeliverDate)
        {
            string depCode = _context.Department.FirstOrDefault(x => x.DepartmentName == departmentName).DepartmentCode;
            var disbursementLists = _context.DisbursementList.Where(x => x.DepartmentCode == depCode && x.Status == "Pending Delivery").ToList();
            foreach (var dl in disbursementLists)
            {
                dl.EstDeliverDate = estDeliverDate;
                _context.SaveChanges();
            }
        }

        //Adjustment details
        public List<AdjustmentRecordDetails> GetAdjustmentRecordDetails(string voucherNo)
        {
            List<RecordDetails> rd = new List<RecordDetails>();
            AdjustmentRecord ar = _context.AdjustmentRecord
                .FirstOrDefault(x => x.VoucherNo == voucherNo && !x.VoucherNo.Contains("VTemp"));
            rd = _context.RecordDetails.Where(x => x.Rrid == ar.VoucherNo).ToList();

            List<AdjustmentRecordDetails> result = new List<AdjustmentRecordDetails>();
            int rowID = 1;
            foreach (var item in rd)
            {
                AdjustmentRecordDetails arList = new AdjustmentRecordDetails();
                arList.RowID = rowID;
                arList.RDID = item.Rdid;
                arList.VoucherNo = item.Rrid;
                arList.ItemNumber = item.ItemNumber;
                arList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                arList.Quantity = item.Quantity;
                arList.Remark = item.Remark;

                result.Add(arList);
                rowID++;
            }
            return result;
        }

        //Draft voucher details
        public List<TempVoucherDetails> GetTempVoucherDetailsList(int userId)
        {
            List<TempVoucherDetails> result = new List<TempVoucherDetails>();
            AdjustmentRecord ar = _context.AdjustmentRecord
                .FirstOrDefault(x => x.ClerkId == userId && x.VoucherNo.Contains("VTemp"));
            if (ar != null)
            {
                List<RecordDetails> rdList = _context.RecordDetails.Where(x => x.Rrid == ar.VoucherNo).ToList();
                int rowID = 1;
                foreach (var item in rdList)
                {
                    TempVoucherDetails tvList = new TempVoucherDetails();
                    tvList.RowID = rowID;
                    tvList.RDID = item.Rdid;
                    tvList.ItemNumber = item.ItemNumber;
                    tvList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                    tvList.Quantity = item.Quantity;
                    tvList.Price = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).Supplier1Price;
                    tvList.Remark = item.Remark;

                    result.Add(tvList);
                    rowID++;
                }
            }
            return result;
        }

        //Create New VoucherItem
        public void CreateNewVoucherItem(int userId, string itemNumber, int qty, string remark)
        {
            string voucherNo = "VTemp" + userId.ToString();
            AdjustmentRecord adjustmentRecord = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == voucherNo);

            if (adjustmentRecord == null)
            {
                adjustmentRecord = new AdjustmentRecord()
                {
                    VoucherNo = voucherNo,
                    IssueDate = DateTime.Today,
                    ClerkId = userId,
                    Status = "Draft"
                };
                _context.AdjustmentRecord.Add(adjustmentRecord);
                _context.SaveChanges();
            }

            RecordDetails recordDetails = new RecordDetails();
            recordDetails.Rrid = voucherNo;
            recordDetails.ItemNumber = itemNumber;
            recordDetails.Quantity = qty;
            recordDetails.Remark = remark;
            _context.RecordDetails.Add(recordDetails);
            _context.SaveChanges();

        }

        public void CreateNewVoucherItem(int userId, string voucherNo, string itemNumber, int qty, string remark)
        {           
            AdjustmentRecord adjustmentRecord = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == voucherNo);

            if (adjustmentRecord == null)
            {
                adjustmentRecord = new AdjustmentRecord()
                {
                    VoucherNo = voucherNo,
                    IssueDate = DateTime.Today,
                    ClerkId = userId,
                    Status = "draft"
                };
                _context.AdjustmentRecord.Add(adjustmentRecord);
                _context.SaveChanges();
            }

            RecordDetails recordDetails = new RecordDetails();
            recordDetails.Rrid = voucherNo;
            recordDetails.ItemNumber = itemNumber;
            recordDetails.Quantity = qty;
            recordDetails.Remark = remark;
            _context.RecordDetails.Add(recordDetails);
            _context.SaveChanges();

        }

        //Update voucherItem
        public void UpdateVoucherItem(int rowID, int quantity, string remark, List<TempVoucherDetails> tempVoucherDetailsList)
        {
            //get rdid
            TempVoucherDetails tempVoucherItem = tempVoucherDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempVoucherItem.RDID;

            RecordDetails editVoucherItem = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            editVoucherItem.Quantity = quantity;
            editVoucherItem.Remark = remark;
            _context.Update(editVoucherItem);
            _context.SaveChanges();
        }

        public void UpdateVoucherItem(int rowID, int quantity, string remark, List<AdjustmentRecordDetails> adjustmentRecordDetailsList)
        {
            //get rdid
            AdjustmentRecordDetails adjustmentRecordItem = adjustmentRecordDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = adjustmentRecordItem.RDID;

            RecordDetails editVoucherItem = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            editVoucherItem.Quantity = quantity;
            editVoucherItem.Remark = remark;
            _context.Update(editVoucherItem);
            _context.SaveChanges();
        }

        //CreateAdjustmentRecord
        public void CreateAdjustmentRecord(int userID, string voucherNo, string status)
        {

            if (status == "Pending Approval")
            {
                //Generate new adjustment record
                AdjustmentRecord ar = new AdjustmentRecord();
                ar.VoucherNo = voucherNo;
                ar.IssueDate = DateTime.Now.Date;
                ar.ClerkId = userID;
                ar.Status = "Pending Approval";

                _context.AdjustmentRecord.Add(ar);
                _context.SaveChanges();
            }
            else if (status == "Draft")
            {
                //Generate new adjustment record
                AdjustmentRecord ar = new AdjustmentRecord();
                ar.VoucherNo = voucherNo;
                ar.IssueDate = DateTime.Now.Date;
                ar.ClerkId = userID;
                ar.Status = "Draft";

                _context.AdjustmentRecord.Add(ar);
                _context.SaveChanges();
            }
        }

        //Add voucher items to voucher
        public void AddItemsToVoucher(int rowID, string voucherNo, List<TempVoucherDetails> tempVoucherDetailsList)
        {
            //get rdid
            TempVoucherDetails tempVoucherItem = tempVoucherDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempVoucherItem.RDID;

            //update voucher issue date
            var voucher = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == voucherNo);
            if(voucher != null)
            {
                voucher.IssueDate = DateTime.Now.Date;
                _context.AdjustmentRecord.Update(voucher);
                _context.SaveChanges();
            }

            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                rd.Rrid = voucherNo;
                _context.SaveChanges();
            }
        }

        //Delete voucher item
        public void DeleteVoucherItem(int rowID, List<TempVoucherDetails> tempVoucherDetailsList)
        {
            //get rdid
            TempVoucherDetails tempVoucherItem = tempVoucherDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempVoucherItem.RDID;

            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                _context.RecordDetails.Remove(rd);
                _context.SaveChanges();
            }
        }

        public void DeleteVoucherItem(int rdid, List<AdjustmentRecordDetails> adjustmentRecordDetailsList)
        {
            //get rdid
            AdjustmentRecordDetails adjustmentRecordItem = adjustmentRecordDetailsList.FirstOrDefault(x => x.RDID == rdid);
            //int rdid = adjustmentRecordItem.RDID;

            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                _context.RecordDetails.Remove(rd);
                _context.SaveChanges();
            }
        }

        //Update adjustment record status
        public void UpdateRecordStatus(string status, string recordName, string rrid)
        {
            if (recordName == "AdjustmentRecord")
            {
                AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == rrid);
                ar.Status = status;
                ar.IssueDate = DateTime.Now.Date;
                _context.AdjustmentRecord.Update(ar);
                _context.SaveChanges();
            }
        }

        //copy model of adjustment record to view model of adjustment record
        public List<AdjustmentRecordViewModel> CreateAdjustmentRecordViewModel(List<AdjustmentRecord> arList)
        {
            List<AdjustmentRecordViewModel> arViewModel = new List<AdjustmentRecordViewModel>();
            if (arList.Count != 0)
            {
                foreach(var q in arList)
                {
                    AdjustmentRecordViewModel ar = new AdjustmentRecordViewModel();
                    ar.VoucherNo = q.VoucherNo;
                    ar.IssueDate = q.IssueDate;
                    ar.ApproveDate = q.ApproveDate;
                    ar.ClerkId = q.ClerkId;
                    ar.ClerkName = _context.User.FirstOrDefault(x => x.UserId == q.ClerkId).Name;
                    ar.SuperviserId = q.SuperviserId;
                    if(q.SuperviserId != null)
                    {
                        ar.SupervisorName = _context.User.FirstOrDefault(x => x.UserId == q.SuperviserId).Name;
                    }
                    else
                    {
                        ar.SupervisorName = "";
                    }
                    
                    ar.ManagerId = q.ManagerId;
                    if(q.ManagerId != null)
                    {
                        ar.ManagerName = _context.User.FirstOrDefault(x => x.UserId == q.ManagerId).Name;
                    }
                    else
                    {
                        ar.ManagerName = "";
                    }
                    
                    ar.Status = q.Status;

                    arViewModel.Add(ar);
                }
            }

            return arViewModel;
        }


        //Reject Voucher
        public void RejectVoucher(int userID, string userRole, string voucherNo)
        {
            var ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == voucherNo);
            if (ar != null)
            {
                if (userRole == "Supervisor")
                {
                    ar.SuperviserId = 0;
                    ar.Status = "Rejected";
                    ar.ApproveDate = DateTime.Now.Date;
                    _context.AdjustmentRecord.Update(ar);
                    _context.SaveChanges();
                }
                else if (userRole == "Manager")
                {
                    ar.ManagerId = 0;
                    ar.Status = "Rejected";
                    ar.ApproveDate = DateTime.Now.Date;
                    _context.AdjustmentRecord.Update(ar);
                    _context.SaveChanges();
                }
            }
        }

        //ApproveVoucher
        public void ApproveVoucher(int userID, string userRole, string voucherNo)
        {            
            var ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == voucherNo);
            if (ar != null)
            {
                var arList = _context.RecordDetails.Where(x => x.Rrid == voucherNo);
                if (arList != null)
                {
                    //value of voucher
                    decimal? amount = GetTotalAmountForVoucher(voucherNo);
                    decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
                    decimal? totalAmount = amount + GST;
                    
                    if (userRole == "Supervisor")
                    {
                        if (totalAmount <= 250)
                        {
                            ar.Status = "Approved";
                            ar.SuperviserId = userID;
                            ar.ApproveDate = DateTime.Now.Date;
                            _context.AdjustmentRecord.Update(ar);
                            _context.SaveChanges();

                            foreach (var item in arList.ToList())
                            {
                                UpdateCatalogueStockAfterSuppDeliveryOrVoucherApproved(item.ItemNumber, item.Quantity);
                                int balance = _context.Catalogue.Find(item.ItemNumber).Stock;
                                UpdateInventoryTransRecord(item.ItemNumber, voucherNo, item.Quantity, balance);
                            }                            
    
                    }
                        else if (totalAmount > 250)
                        {
                            ar.Status = "Pending Manager Approval";
                            ar.SuperviserId = userID;
                            ar.ApproveDate = DateTime.Now.Date;
                            _context.AdjustmentRecord.Update(ar);
                            _context.SaveChanges();
                        }
                    }
                    else if (userRole == "Manager")
                    {
                        ar.Status = "Approved";
                        ar.ManagerId = userID;
                        ar.ApproveDate = DateTime.Now.Date;
                        _context.AdjustmentRecord.Update(ar);
                        _context.SaveChanges();

                        foreach (var item in arList.ToList())
                        {
                            UpdateCatalogueStockAfterSuppDeliveryOrVoucherApproved(item.ItemNumber, item.Quantity);
                            int balance = _context.Catalogue.Find(item.ItemNumber).Stock;
                            UpdateInventoryTransRecord(item.ItemNumber, voucherNo, item.Quantity, balance);
                        }
                    }
                }               
            }            
        }

        //Update inventory transaction after voucher approved
        public void UpdateTransAfterVoucherApproved()
        {
        }

        //Amount(ex. GST) of a voucher, supplier1Price is used
        public decimal? GetTotalAmountForVoucher(string voucherNo)
        {
            var tmp = _context.RecordDetails.Where(s => s.Rrid == voucherNo).ToList();
            if (tmp == null)
                return 0;
            decimal? price = 0;
            foreach (RecordDetails i in tmp)
            {
                Catalogue k = _context.Catalogue.Where(s => s.ItemNumber == i.ItemNumber).ToList().First();
                decimal? p = Math.Abs(i.Quantity) * k.Supplier1Price;
                price += p;
            }
            return price;
        }

        //Amount(ex. GST) of a voucher, supplier1Price is used
        public decimal? GetTotalAmountForPO(string poid)
        {
            var tmp = _context.RecordDetails.Where(s => s.Rrid == poid).ToList();
            if (tmp == null)
                return 0;
            decimal? price = 0;
            string supplierCode = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poid).SupplierCode;
            foreach (RecordDetails i in tmp)
            {

                decimal? pricePerItem = GetPriceOfItem(i.ItemNumber, supplierCode);
                decimal? p = i.Quantity * pricePerItem;
                price += p;
            }
            return price;
        }

        //FindDepartmentOrSupplier through disbursement list ID or PO ID
        public string FindDepartmentOrSupplier(string recordId)
        {
            string result = "";

            try
            {
                var findDep = _context.DisbursementList.FirstOrDefault(x => x.Dlid == recordId).DepartmentCode;

                if (findDep != null)
                {
                    result = _context.Department.Find(findDep).DepartmentName + " Department";
                }
            }
            catch (NullReferenceException)
            {

            }
            try
            {
                var findSupp = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == recordId).SupplierCode;
                if (findSupp != null)
                {
                    result = "Supplier - " + _context.Supplier.Find(findSupp).SupplierName;
                }
            }
            catch (NullReferenceException)
            {

            }
            try
            {
                var findAdjustment = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == recordId);
                if (findAdjustment != null)
                {
                    result = "Stock Adjustment " + recordId;
                }
            }
            catch (NullReferenceException)
            {

            }
            return result;
        }

        //Get 
        public List<PurchaseOrderRecordDetails> GetPurchaseOrderRecordDetails(string poid)
        {
            List<RecordDetails> rd = new List<RecordDetails>();
            PurchaseOrderRecord purchaseOrderRecord = _context.PurchaseOrderRecord
                .FirstOrDefault(x => x.Poid == poid && !x.Poid.Contains("POTemp"));
            rd = _context.RecordDetails.Where(x => x.Rrid == purchaseOrderRecord.Poid).ToList();

            List<PurchaseOrderRecordDetails> result = new List<PurchaseOrderRecordDetails>();
            int rowID = 1;
            foreach (var item in rd)
            {
                PurchaseOrderRecordDetails poList = new PurchaseOrderRecordDetails();

                poList.RowID = rowID;
                poList.ItemNumber = item.ItemNumber;
                poList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                poList.Quantity = item.Quantity;
                poList.QuantityDelivered = item.QuantityDelivered;
                string supplierCode = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poid).SupplierCode;
                poList.Price = GetPriceOfItem(item.ItemNumber, supplierCode);
                poList.RDID = item.Rdid;
                poList.POID = poid;

                result.Add(poList);
                rowID++;
            }
            return result;
        }


        //Add item at reorder qty to TempPurchaseOrderDetailsList
        public void AddReorderLevelItemToTempPurchaseOrderDetailsList()
        {
            var itemsAtReorderLevel = _context.Catalogue.Where(x => x.Stock <= x.ReorderLevel).ToList();

            //find pending delivery items quantity
            var pendingDeliveryOrSubmitItems = from rd in _context.RecordDetails
                                       join po in _context.PurchaseOrderRecord on rd.Rrid equals po.Poid
                                       where po.Status == "Pending Delivery" || po.Status == "Draft"
                                       group rd by new { rd.ItemNumber } into g
                                       select new { g.Key.ItemNumber, Quantity = g.Sum(x => x.Quantity) };

            List<TempPurchaseOrderDetails> autoPurchaseOrderDetails = new List<TempPurchaseOrderDetails>();            
            if(itemsAtReorderLevel != null)
            {
                int rowID = 1;
                foreach (var item in itemsAtReorderLevel)
                {
                    var q = pendingDeliveryOrSubmitItems.FirstOrDefault(x => x.ItemNumber == item.ItemNumber && x.Quantity >= item.ReorderQty);
                    if(q == null)
                    {
                        //update tempPOdetails
                        //TempPurchaseOrderDetails tempPOD = new TempPurchaseOrderDetails();
                        //tempPOD.RowID = rowID;
                        //tempPOD.ItemNumber = item.ItemNumber;
                        //tempPOD.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                        //tempPOD.Quantity = item.ReorderQty;
                        //tempPOD.Remark = "";
                        string supplierCode = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).Supplier1;
                        //tempPOD.SupplierCode = supplierCode;
                        //tempPOD.Price = GetPriceOfItem(item.ItemNumber, supplierCode);

                        //autoPurchaseOrderDetails.Add(tempPOD);
                        //rowID++;

                        //add to POTemp
                        RecordDetails autoAddItem = new RecordDetails();
                        CreateNewPOItem(0, item.ItemNumber, item.ReorderQty, supplierCode);
                    }
                }
            }
        }

        //Get temp PurchaseOrderDetailsList
        public List<TempPurchaseOrderDetails> GetTempPurchaseOrderDetailsList()
        {
            List<TempPurchaseOrderDetails> result = new List<TempPurchaseOrderDetails>();

            //Find auto generated items by system
            AddReorderLevelItemToTempPurchaseOrderDetailsList();

            //Find manual add in items via "POTemp"
            List<PurchaseOrderRecord> purchaseOrderRecordList = _context.PurchaseOrderRecord
                .Where(x => x.Poid.Contains("POTemp")).ToList();
            List<TempPurchaseOrderDetails> manualPurchaseOrderDetails = new List<TempPurchaseOrderDetails>();
            //last row ID of auto
            int rowID = 1;
            //if (autoPurchaseOrderDetails.Count != 0)
            //{
            //    rowID = autoPurchaseOrderDetails.Select(x => x.RowID).Max() + 1;
            //}
            //else
            //{
            //    rowID = 1;
            //}
            if (purchaseOrderRecordList.Count != 0)
            {
                foreach (var record in purchaseOrderRecordList)
                {
                    List<RecordDetails> rdList = _context.RecordDetails.Where(x => x.Rrid == record.Poid).ToList();
                    foreach (var item in rdList)
                    {
                        TempPurchaseOrderDetails tPOList = new TempPurchaseOrderDetails();
                        tPOList.RowID = rowID;
                        tPOList.RDID = item.Rdid;
                        tPOList.ItemNumber = item.ItemNumber;
                        tPOList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                        tPOList.Quantity = item.Quantity;
                        tPOList.Remark = item.Remark;
                        string supplierCode = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == item.Rrid).SupplierCode;
                        tPOList.SupplierCode = supplierCode;
                        tPOList.Price = GetPriceOfItem(item.ItemNumber, supplierCode);

                        manualPurchaseOrderDetails.Add(tPOList);
                        rowID++;
                    }
                }
            }

            //join auto and manual generated tempPOlist
            //result = autoPurchaseOrderDetails.Concat(manualPurchaseOrderDetails).ToList<TempPurchaseOrderDetails>();
            result = manualPurchaseOrderDetails;
            return result;
        }

        //find price according to supplier code
        public decimal? GetPriceOfItem(string itemNumber, string supplierCode)
        {
            decimal? price = 0;
            string supplier1 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier1;
            string supplier2 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier2;
            string supplier3 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier3;
            if (supplierCode == supplier1)
            { 
                price = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier1Price;
            }
            else if (supplierCode == supplier2)
            {
                price = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier2Price;
            }
            else if (supplierCode == supplier3)
            {
                price = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier3Price;
            }
            return price;
        }

        //Create New POItem
        public void CreateNewPOItem(int userID, string itemNumber, int qty, string supplierCode)
        {
            //string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
            string poNo = "POTemp" + supplierCode;
            PurchaseOrderRecord purchaseOrderRecord = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poNo && x.SupplierCode == supplierCode);

            if (purchaseOrderRecord == null)
            {
                PurchaseOrderRecord poRecord = new PurchaseOrderRecord()
                {
                    Poid = poNo,
                    OrderDate = DateTime.Today,
                    StoreClerkId = userID,
                    SupplierCode = supplierCode,
                    Status = "Draft"
                };
                _context.PurchaseOrderRecord.Add(poRecord);
                _context.SaveChanges();
            }

            //check if item exists
            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rrid == poNo && x.ItemNumber == itemNumber);
            if (rd == null)
            {
                RecordDetails recordDetails = new RecordDetails();
                recordDetails.Rrid = poNo;
                recordDetails.ItemNumber = itemNumber;
                recordDetails.Quantity = qty;
                recordDetails.Remark = "";
                _context.RecordDetails.Add(recordDetails);
                _context.SaveChanges();
            }
            else
            {
                rd.Quantity += qty;
                _context.RecordDetails.Update(rd);
                _context.SaveChanges();
            }            

        }

        //Update POItem
        public void UpdatePOItem(int userID, int rowID, int quantity, string supplierCode, List<TempPurchaseOrderDetails> tempPurchaseOrderDetails)
        {
            //get rdid
            TempPurchaseOrderDetails tempPOItem = tempPurchaseOrderDetails.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempPOItem.RDID;
            string poidOld = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid).Rrid;
            //string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
            string poidNew = "POTemp" + supplierCode;
            //Check if PO ID exists
            PurchaseOrderRecord purchaseOrderRecord = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poidNew);

            //Update PO Record
            if (purchaseOrderRecord == null)
            {
                PurchaseOrderRecord poRecord = new PurchaseOrderRecord()
                {
                    Poid = poidNew,
                    OrderDate = DateTime.Today,
                    StoreClerkId = userID,
                    SupplierCode = supplierCode,
                    Status = "Draft"
                };
                _context.PurchaseOrderRecord.Add(poRecord);
                _context.SaveChanges();
            }

            //Update recordDetails
            RecordDetails editPOItem = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            editPOItem.Quantity = quantity;
            editPOItem.Rrid = poidNew;
            _context.RecordDetails.Update(editPOItem);
            _context.SaveChanges();
        }

        public void UpdatePOItemQtyOrdered(int rdid, int quantity)
        {
            var record = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            record.Quantity = quantity;
            _context.RecordDetails.Update(record);
            _context.SaveChanges();
        }

        //CreatePurchaseOrderRecord
        public void CreatePurchaseOrderRecord(int userID, string poNo, string supplierCode, string status)
        {
            //string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
            PurchaseOrderRecord poRecord = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poNo);
            if (poRecord == null && supplierCode != null)
            {
                //Generate new adjustment record
                PurchaseOrderRecord purchaseOrderRecord = new PurchaseOrderRecord();
                purchaseOrderRecord.Poid = poNo;
                purchaseOrderRecord.OrderDate = DateTime.Now.Date;
                purchaseOrderRecord.StoreClerkId = userID;
                purchaseOrderRecord.SupplierCode = supplierCode;
                purchaseOrderRecord.Status = status;

                _context.PurchaseOrderRecord.Add(purchaseOrderRecord);
                _context.SaveChanges();
            }
        }

        //Add items to PO
        public void AddItemsToPO(int rowID, string poNo, List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList)
        {
           
            TempPurchaseOrderDetails tempPurchaseOrderDetails = tempPurchaseOrderDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempPurchaseOrderDetails.RDID;
            if(rdid != 0)
            {
                //for existing items in records, get rdid
                //update record details
                RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
                if (rd != null)
                {
                    rd.Rrid = poNo;
                    _context.SaveChanges();
                }
            }
            else
            {
                //for non-existing items in records
                RecordDetails rd = new RecordDetails();
                rd.Rrid = poNo;
                rd.ItemNumber = tempPurchaseOrderDetails.ItemNumber;
                rd.Quantity = tempPurchaseOrderDetails.Quantity;
                rd.QuantityDelivered = 0;
                _context.RecordDetails.Add(rd);
                _context.SaveChanges();
            }           
        }

        //Delete PO item
        public void DeletePOItem(int rowID, List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList)
        {
            //get rdid
            TempPurchaseOrderDetails tempPurchaseOrderDetails = tempPurchaseOrderDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempPurchaseOrderDetails.RDID;

            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                _context.RecordDetails.Remove(rd);
                _context.SaveChanges();
            }
        }

        public void DeletePOItem(int rowID, List<PurchaseOrderRecordDetails> purchaseOrderDetailsList)
        {
            //get rdid
            PurchaseOrderRecordDetails purchaseOrderDetails = purchaseOrderDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = purchaseOrderDetails.RDID;

            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                _context.RecordDetails.Remove(rd);
                _context.SaveChanges();
            }
        }

        //RemoveRecordDetails
        public void RemoveRecordDetails(string id)
        {
            List<RecordDetails> recordDetailsToBeDeleted = _context.RecordDetails.Where(x => x.Rrid == id).ToList();
            foreach (var q in recordDetailsToBeDeleted)
            {
                _context.RecordDetails.Remove(q);
                _context.SaveChanges();
                
            }
        }

        //Remove PO record
        public void RemovePORecord(string id)
        {
            PurchaseOrderRecord poRecordToBeDeleted = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == id);
            _context.PurchaseOrderRecord.Remove(poRecordToBeDeleted);
            _context.SaveChanges();
        }

        //UpdateCatalogueStockAfterSupplierDelivery
        public void UpdateCatalogueStockAfterSuppDeliveryOrVoucherApproved(string itemNumber, int quantity)
        {
            Catalogue item = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber);
            int stock = item.Stock;
            item.Stock = stock + quantity;
            if(item.Stock >= 0)
            {
                _context.Catalogue.Update(item);
                _context.SaveChanges();
            }            
        }

        //Department part
        public List<EmployeeRequestRecord> searchRequestByDateAndDept(DateTime startDate, DateTime endDate, string dept)
        {
            var t = _context.EmployeeRequestRecord.Where(s => s.RequestDate >= startDate && s.RequestDate <= endDate && s.DepCode == dept);
            List<EmployeeRequestRecord> list = new List<EmployeeRequestRecord>();
            list = t.ToList();
            return list;
        }

        public List<EmployeeRequestRecord> searchRequestByDept(string dept)
        {
            return _context.EmployeeRequestRecord.Where(x => x.DepCode == dept).ToList();
        }

        public EmployeeRequestRecord searchEmployeeRequestByRRID(string rrid)
        {
            var q1 = _context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = new EmployeeRequestRecord();
            e1 = q1;
            return e1;
        }

        public List<RecordDetails> searchRecordDetailsByRRID(string rrid)
        {
            return _context.RecordDetails.Where(x => x.Rrid == rrid).ToList();
        }

        public DisbursementList searchDLByPendingDeliveryAndDept(string dept)
        {
            var q = _context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Pending Delivery").First();
            DisbursementList d = new DisbursementList();
            d = q;
            return d;
        }
        public string getCollectionPointName(int currentCollectionPoint)
        {
            var q2 = _context.CollectionPoint.Where(x => x.CollectionPointId == currentCollectionPoint).First();
            CollectionPoint c1 = q2;
            string currentName = c1.CollectionPointName;
            return currentName;
        }
        public List<CollectionPoint> populateCPDropDownList(int currentCollectionPoint)
        {
            return _context.CollectionPoint.Where(x => x.CollectionPointId != currentCollectionPoint).ToList();
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public List<EmployeeRequestRecord> searchOutstandingRequests(string dept)
        {
            var q = _context.EmployeeRequestRecord.Where(x => x.Status == "Pending Approval" && x.DepCode == dept);
            return q.ToList();
        }

        public string getEmpName(int depEmpId)
        {
            var q = _context.User.Where(x => x.UserId == depEmpId).First();
            string name = q.Name;
            return name;
        }

        //this part is temp and may not work on other status
        public string getPriceForAdjust(string VoNo)
        {
            List<RecordDetails> tmp = _context.RecordDetails.Where(s => s.Rrid == VoNo).ToList();
            if (tmp == null)
                return "0";
            double price = 0;
            foreach(RecordDetails i in tmp)
            {
                Catalogue k = _context.Catalogue.Where(s => s.ItemNumber == i.ItemNumber).ToList().First();
                double p = Math.Abs( i.Quantity)*(double)k.Supplier1Price;
                price += p;
            }
            return price.ToString();
        }


        


        //Generate Report Part
        //Get Stationery Usage of all completed disbursement lists
        public List<StationeryUsageViewModel> GetStationeryUsage(string status)
        {
            List<StationeryUsageViewModel> stationeryUsageViewModelList = new List<StationeryUsageViewModel>();
            
            //join disbursement lists and record details table
            var q = from rd in _context.RecordDetails join dl in _context.DisbursementList on rd.Rrid equals dl.Dlid
                    where dl.Status == status
                    select new { category = rd.ItemNumberNavigation.Category, rd.QuantityDelivered, dl.CompleteDate, dl.DepartmentCode };

            if (q != null)
            {
                int rowID = 1;
                foreach (var item in q.ToList())
                {
                    StationeryUsageViewModel stationeryUsageViewModel = new StationeryUsageViewModel();
                    stationeryUsageViewModel.RowID = rowID;
                    stationeryUsageViewModel.Category = item.category;
                    stationeryUsageViewModel.QuantityDelivered = item.QuantityDelivered;
                    stationeryUsageViewModel.CompleteDate = item.CompleteDate;
                    stationeryUsageViewModel.Year = item.CompleteDate.Value.Year;
                    stationeryUsageViewModel.Month = item.CompleteDate.Value.Month;
                    stationeryUsageViewModel.DepCode = item.DepartmentCode;
                    
                    stationeryUsageViewModelList.Add(stationeryUsageViewModel);
                    rowID++;
                }
            }


            return stationeryUsageViewModelList;
        }

        //Get Stationery Usage of all selected disbursement lists
        public List<StationeryUsageViewModel> GetStationeryUsage(string status, DateTime startDate, DateTime endDate, List<string> yearNames, List<string> monthNames, List<string> departmentCodes, List<string> categoryNames)
        {
            List<StationeryUsageViewModel> stationeryUsageViewModelList = new List<StationeryUsageViewModel>();

            //join disbursement lists and record details table
            var filterByStatus = from rd in _context.RecordDetails
                    join dl in _context.DisbursementList on rd.Rrid equals dl.Dlid
                    where dl.Status == status
                    orderby dl.CompleteDate ascending
                    select new { category = rd.ItemNumberNavigation.Category, rd.QuantityDelivered, dl.CompleteDate,
                        year = dl.CompleteDate.Value.Year, month = dl.CompleteDate.Value.Month, dl.DepartmentCode };

            var groupByYearMonth = filterByStatus.GroupBy( x => new { x.category, x.year, x.month, x.DepartmentCode })
                .Select(x => new { Category = x.Key.category, Departmentcode = x.Key.DepartmentCode,
                    Year = x.Key.year, Month = x.Key.month, Quantity = x.Sum(y=>y.QuantityDelivered)});


            List<StationeryUsageViewModel> filterByDepAndCat = new List<StationeryUsageViewModel>();
            List<StationeryUsageViewModel> filterByYearMonth = new List<StationeryUsageViewModel>();
            if (groupByYearMonth != null)
            {
                //select department and category
                if(departmentCodes != null && categoryNames != null)
                {
                   foreach(var item in groupByYearMonth.ToList())
                    {
                        int rowID = 1;
                        if (departmentCodes.Contains(item.Departmentcode) && categoryNames.Contains(item.Category))
                        {
                            StationeryUsageViewModel stationeryUsageViewModel = new StationeryUsageViewModel();
                            stationeryUsageViewModel.Category = item.Category;
                            stationeryUsageViewModel.QuantityDelivered = item.Quantity;
                            stationeryUsageViewModel.Year = item.Year;
                            stationeryUsageViewModel.Month = item.Month;
                            stationeryUsageViewModel.DepCode = item.Departmentcode;

                            filterByDepAndCat.Add(stationeryUsageViewModel);
                            rowID++;
                        }
                    }
                    if (yearNames != null && monthNames != null)
                    {
                        foreach (var item in filterByDepAndCat)
                        {
                            int rowID = 1;
                            if (yearNames.Contains(item.Year.ToString()) && monthNames.Contains(item.Month.ToString()) )
                            {
                                StationeryUsageViewModel stationeryUsageViewModel = new StationeryUsageViewModel();
                                stationeryUsageViewModel.Category = item.Category;
                                stationeryUsageViewModel.QuantityDelivered = item.QuantityDelivered;
                                stationeryUsageViewModel.Year = item.Year;
                                stationeryUsageViewModel.Month = item.Month;
                                stationeryUsageViewModel.DepCode = item.DepCode;

                                filterByYearMonth.Add(stationeryUsageViewModel);
                                rowID++;
                            }
                        }
                        stationeryUsageViewModelList = filterByYearMonth;
                    }
                }
            }

            return stationeryUsageViewModelList;
        }


        //GetChargeBack
        public List<ChargeBackViewModel> GetChargeBack(string status)
        {
            List<ChargeBackViewModel> chargeBackViewModelsList = new List<ChargeBackViewModel>();

            //join disbursement lists and record details table
            var q = from rd in _context.RecordDetails
                    join dl in _context.DisbursementList on rd.Rrid equals dl.Dlid
                    where dl.Status == status
                    select new { rd.ItemNumber, rd.QuantityDelivered, dl.CompleteDate, dl.DepartmentCode };

            if (q != null)
            {
                int rowID = 1;
                foreach (var item in q.ToList())
                {
                    ChargeBackViewModel chargeBackViewModel = new ChargeBackViewModel();
                    decimal? supplier1Price = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).Supplier1Price;

                    chargeBackViewModel.RowID = rowID;
                    chargeBackViewModel.Year = item.CompleteDate.Value.Year;
                    chargeBackViewModel.Month = item.CompleteDate.Value.Month;
                    chargeBackViewModel.DepCode = item.DepartmentCode;
                    chargeBackViewModel.TotalAmount = item.QuantityDelivered * supplier1Price;

                    chargeBackViewModelsList.Add(chargeBackViewModel);
                    rowID++;
                }
            }

            return chargeBackViewModelsList;
        }

        public List<ChargeBackViewModel> GetChargeBack(string status, DateTime startDate, DateTime endDate, List<string> yearNames, List<string> monthNames, List<string> departmentCodes)
        {
            List<ChargeBackViewModel> chargeBackViewModelsList = new List<ChargeBackViewModel>();

            //join disbursement lists and record details table
            var filterByStatus = from rd in _context.RecordDetails
                                 join dl in _context.DisbursementList on rd.Rrid equals dl.Dlid
                                 where dl.Status == status
                                 orderby dl.CompleteDate ascending
                                 select new
                                 {
                                     itemNumber = rd.ItemNumber,
                                     rd.QuantityDelivered,
                                     dl.CompleteDate,
                                     year = dl.CompleteDate.Value.Year,
                                     month = dl.CompleteDate.Value.Month,
                                     dl.DepartmentCode
                                 };

            //var groupByYearMonthItem1 = filterByStatus.GroupBy(x => new { x.itemNumber, x.year, x.month, x.DepartmentCode })
            //    .Select(x => new
            //    {
            //        ItemNumber = x.Key.itemNumber,
            //        Departmentcode = x.Key.DepartmentCode,
            //        Year = x.Key.year,
            //        Month = x.Key.month,
            //        Quantity = x.Sum(y => y.QuantityDelivered),
            //        Amount = x.Sum(y => y.QuantityDelivered) * _context.Catalogue.Find(x.Key.itemNumber).Supplier1Price
            //    });

            var groupByYearMonthItem = from fbs in filterByStatus
                    join cat in _context.Catalogue on fbs.itemNumber equals cat.ItemNumber
                    group fbs by new { fbs.itemNumber, fbs.year, fbs.month, fbs.DepartmentCode, cat.Supplier1Price } into g
                    select new { g.Key.itemNumber, g.Key.DepartmentCode, g.Key.year, g.Key.month, Amount = g.Sum(x => x.QuantityDelivered) * g.Key.Supplier1Price };

            //var groupByDep = groupByYearMonthItem.GroupBy(x => new { x.year, x.month, x.DepartmentCode })
            //    .Select(x => new { x.Key.DepartmentCode, x.Key.year, x.Key.month, TotalAmount = x.Sum(y => y.Amount)});
            //var groupByDep = from g in groupByYearMonthItem
            //                  group g by new { g.DepartmentCode, g.year, g.month } into p
            //                  select new { p.Key.DepartmentCode, p.Key.year, p.Key.month, TotalAmount = p.Sum(x => x.Amount) };


            List<ChargeBackViewModel> filterByDep = new List<ChargeBackViewModel>();
            List<ChargeBackViewModel> filterByYearMonth = new List<ChargeBackViewModel>();
            if (groupByYearMonthItem != null && departmentCodes != null)
            {
                foreach (var item in groupByYearMonthItem.ToList())
                {
                    int rowID = 1;                    
                    if (departmentCodes.Contains(item.DepartmentCode))
                    {
                        var q = filterByDep.FirstOrDefault(x => x.DepCode == item.DepartmentCode && x.Year == item.year && x.Month == item.month);
                        if (q == null)
                        {
                            ChargeBackViewModel chargeBackViewModel = new ChargeBackViewModel();

                            chargeBackViewModel.Year = item.year;
                            chargeBackViewModel.Month = item.month;
                            chargeBackViewModel.DepCode = item.DepartmentCode;
                            chargeBackViewModel.TotalAmount = item.Amount;

                            filterByDep.Add(chargeBackViewModel);
                        }
                        else
                        {
                            //ChargeBackViewModel chargeBackViewModel = new ChargeBackViewModel();
                            //chargeBackViewModel.Year = item.year;
                            //chargeBackViewModel.Month = item.month;
                            //chargeBackViewModel.DepCode = item.DepartmentCode;
                            q.TotalAmount += item.Amount;
                        }                        
                        rowID++;
                    }
                }
                if (yearNames != null && monthNames != null)
                {
                    foreach (var item in filterByDep)
                    {
                        int rowID = 1;
                        if (yearNames.Contains(item.Year.ToString()) && monthNames.Contains(item.Month.ToString()))
                        {
                            ChargeBackViewModel chargeBackViewModel = new ChargeBackViewModel();

                            chargeBackViewModel.Year = item.Year;
                            chargeBackViewModel.Month = item.Month;
                            chargeBackViewModel.DepCode = item.DepCode;
                            chargeBackViewModel.TotalAmount = item.TotalAmount;

                            filterByYearMonth.Add(chargeBackViewModel);
                            rowID++;
                        }
                    }
                    chargeBackViewModelsList = filterByYearMonth;
                }

            }
            return chargeBackViewModelsList;
        }
    }    
}

