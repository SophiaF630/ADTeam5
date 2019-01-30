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

                foreach (RecordDetails r in selectederrList)
                {
                    //check if dl record exists
                    //RecordDetails rd = new RecordDetails();
                    var q = _context.RecordDetails.FirstOrDefault(x => x.Rrid == dl.Dlid && x.ItemNumber == r.ItemNumber && x.Remark == null);
                    if (q == null)
                    {
                        RecordDetails rd = new RecordDetails() { Rrid = dl.Dlid, ItemNumber = r.ItemNumber, Quantity = r.Quantity };
                        _context.RecordDetails.Add(rd);
                    }
                    else
                    {
                        RecordDetails rd = new RecordDetails();
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
                int preStock = i.Stock;
                int preOut = i.Out;
                if (preStock >= outQty)
                {
                    i.Out = outQty;
                    i.Stock = preStock - (outQty - preOut);
                    _context.SaveChanges();
                }
                else
                {

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
                else
                {

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
        public void GenerateDisbursementListForPartialFulfillment(string itemNumber, int qty, string remark, string depCode)
        {
            string dlID = IDGenerator("DL");
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
            InventoryTransRecord inventoryTransRecord = new InventoryTransRecord();
            inventoryTransRecord.Date = DateTime.Now.Date;
            inventoryTransRecord.ItemNumber = itemNumber;
            inventoryTransRecord.RecordId = recordID;
            inventoryTransRecord.Qty = qty;
            inventoryTransRecord.Balance = balance;
            _context.InventoryTransRecord.Add(inventoryTransRecord);
            _context.SaveChanges();
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

            if (status == "Submitted")
            {
                //Generate new adjustment record
                AdjustmentRecord ar = new AdjustmentRecord();
                ar.VoucherNo = voucherNo;
                ar.IssueDate = DateTime.Now.Date;
                ar.ClerkId = userID;
                ar.Status = "Submitted";

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
                _context.AdjustmentRecord.Update(ar);
                _context.SaveChanges();
            }
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
            foreach (var item in rd)
            {
                PurchaseOrderRecordDetails poList = new PurchaseOrderRecordDetails();

                poList.ItemNumber = item.ItemNumber;
                poList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                poList.Quantity = item.Quantity;

                result.Add(poList);
            }
            return result;
        }

    //Get temp PurchaseOrderDetailsList
    public List<TempPurchaseOrderDetails> GetTempPurchaseOrderDetailsList()
        {
            List<TempPurchaseOrderDetails> result = new List<TempPurchaseOrderDetails>();
            List<PurchaseOrderRecord> purchaseOrderRecordList = _context.PurchaseOrderRecord
                .Where(x => x.Poid.Contains("POTemp")).ToList();
            if (purchaseOrderRecordList.Count != 0)
            {
                int rowID = 1;
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
                        tPOList.SupplierCode = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == item.Rrid).SupplierCode;

                        result.Add(tPOList);
                        rowID++;
                    }
                }
            }
            return result;
        }

        //Create New VoucherItem
        public void CreateNewPOItem(int userID, string itemNumber, int qty, string supplierName)
        {
            string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
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

            RecordDetails recordDetails = new RecordDetails();
            recordDetails.Rrid = poNo;
            recordDetails.ItemNumber = itemNumber;
            recordDetails.Quantity = qty;
            recordDetails.Remark = "";
            _context.RecordDetails.Add(recordDetails);
            _context.SaveChanges();

        }

        //Update POItem
        public void UpdatePOItem(int userID, int rowID, int quantity, string supplierName, List<TempPurchaseOrderDetails> tempPurchaseOrderDetails)
        {
            //get rdid
            TempPurchaseOrderDetails tempPOItem = tempPurchaseOrderDetails.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempPOItem.RDID;
            string poidOld = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid).Rrid;
            string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
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
            _context.Update(editPOItem);
            _context.SaveChanges();
        }

        //CreatePurchaseOrderRecord
        public void CreatePurchaseOrderRecord(int userID, string poNo, string supplierCode, string status)
        {
            //string supplierCode = _context.Supplier.FirstOrDefault(x => x.SupplierName == supplierName).SupplierCode;
            PurchaseOrderRecord poRecord = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == poNo);
            if (poRecord == null && supplierCode != null)
            {
                if (status == "Submitted")
                {
                    //Generate new adjustment record
                    PurchaseOrderRecord purchaseOrderRecord = new PurchaseOrderRecord();
                    purchaseOrderRecord.Poid = poNo;
                    purchaseOrderRecord.OrderDate = DateTime.Now.Date;
                    purchaseOrderRecord.StoreClerkId = userID;
                    purchaseOrderRecord.SupplierCode = supplierCode;
                    purchaseOrderRecord.Status = "Submitted";

                    _context.PurchaseOrderRecord.Add(purchaseOrderRecord);
                    _context.SaveChanges();
                }
                else if (status == "Draft")
                {
                    //Generate new adjustment record
                    PurchaseOrderRecord purchaseOrderRecord = new PurchaseOrderRecord();
                    purchaseOrderRecord.Poid = poNo;
                    purchaseOrderRecord.OrderDate = DateTime.Now.Date;
                    purchaseOrderRecord.StoreClerkId = userID;
                    purchaseOrderRecord.SupplierCode = supplierCode;
                    purchaseOrderRecord.Status = "Draft";

                    _context.PurchaseOrderRecord.Add(purchaseOrderRecord);
                    _context.SaveChanges();
                }
            }
        }

        //Add items to PO
        public void AddItemsToPO(int rowID, string poNo, List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList)
        {
            //get rdid
            TempPurchaseOrderDetails tempPurchaseOrderDetails = tempPurchaseOrderDetailsList.FirstOrDefault(x => x.RowID == rowID);
            int rdid = tempPurchaseOrderDetails.RDID;
            //update record details
            RecordDetails rd = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid);
            if (rd != null)
            {
                rd.Rrid = poNo;
                _context.SaveChanges();
            }
        }

        //Delete voucher item
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
            var q = _context.EmployeeRequestRecord.Where(x => x.Status == "Submitted" && x.DepCode == dept);
            return q.ToList();
        }

        public string getEmpName(int depEmpId)
        {
            var q = _context.User.Where(x => x.UserId == depEmpId).First();
            string name = q.Name;
            return name;
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

            if (q.ToList().Count != 0)
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
        public List<StationeryUsageViewModel> GetStationeryUsage(string status, DateTime startDate, DateTime endDate, List<int> yearsName, List<int> monthsName, List<string> departmentsCode, List<string> categoriesName)
        {
            List<StationeryUsageViewModel> stationeryUsageViewModelList = new List<StationeryUsageViewModel>();

            //join disbursement lists and record details table
            var q = from rd in _context.RecordDetails
                    join dl in _context.DisbursementList on rd.Rrid equals dl.Dlid
                    where dl.Status == status
                    select new { category = rd.ItemNumberNavigation.Category, rd.QuantityDelivered, dl.CompleteDate, dl.DepartmentCode };

            List<StationeryUsageViewModel> filterByDepAndCat = new List<StationeryUsageViewModel>();
            List<StationeryUsageViewModel> filterByYearMonth = new List<StationeryUsageViewModel>();
            if (q.ToList().Count != 0)
            {
                //select department and category
                if(departmentsCode.Count != 0 && categoriesName.Count != 0)
                {
                   foreach(var item in q.ToList())
                    {
                        int rowID = 1;
                        if (departmentsCode.Contains(item.DepartmentCode) && categoriesName.Contains(item.category))
                        {
                            StationeryUsageViewModel stationeryUsageViewModel = new StationeryUsageViewModel();
                            stationeryUsageViewModel.Category = item.category;
                            stationeryUsageViewModel.QuantityDelivered = item.QuantityDelivered;
                            stationeryUsageViewModel.CompleteDate = item.CompleteDate;
                            stationeryUsageViewModel.Year = item.CompleteDate.Value.Year;
                            stationeryUsageViewModel.Month = item.CompleteDate.Value.Month;
                            stationeryUsageViewModel.DepCode = item.DepartmentCode;

                            filterByDepAndCat.Add(stationeryUsageViewModel);
                            rowID++;
                        }
                    }
                    if (yearsName.Count == 0 || monthsName.Count == 0)
                    {
                        var filterByStartEnd = filterByDepAndCat.Where(x => x.CompleteDate >= startDate && x.CompleteDate <= endDate);
                        stationeryUsageViewModelList = filterByStartEnd.ToList();
                    }
                    else if (yearsName.Count != 0 && monthsName.Count != 0)
                    {
                        foreach (var item in filterByDepAndCat)
                        {
                            int rowID = 1;
                            if ( yearsName.Contains(item.Year) && monthsName.Contains(item.Month) )
                            {
                                StationeryUsageViewModel stationeryUsageViewModel = new StationeryUsageViewModel();
                                stationeryUsageViewModel.Category = item.Category;
                                stationeryUsageViewModel.QuantityDelivered = item.QuantityDelivered;
                                stationeryUsageViewModel.CompleteDate = item.CompleteDate;
                                stationeryUsageViewModel.Year = item.CompleteDate.Value.Year;
                                stationeryUsageViewModel.Month = item.CompleteDate.Value.Month;
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
    }
}
