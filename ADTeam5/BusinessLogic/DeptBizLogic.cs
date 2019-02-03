using ADTeam5.Models;
using ADTeam5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.BusinessLogic
{
    public class DeptBizLogic
    {

        private readonly SSISTeam5Context context = new SSISTeam5Context();

        //AssignDepartment
        public Models.Department getDepartmentDetails(string dept)
        {
            var q1 = context.Department.Where(x => x.DepartmentCode == dept).First();
            Models.Department d = q1;
            return d;
        }
        public Models.User getUser(int userid)
        {
            var q = context.User.Where(x => x.UserId == userid).First();
            Models.User u = q;
            return u;
        }

        public List<User> populateAssignDepartmentDropDownList(string dept, int repid, int headid, int coverheadid)
        {
            var q = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid && x.UserId != coverheadid).OrderBy(x => x.Name).ToList();
            List<User> u = new List<User>();
            u = q;
            return u;
        }

        //AssignDeputy
        public string getCurrentDeputyHeadName(int currentDeputyHeadId)
        {
            var q = context.User.Where(x => x.UserId == currentDeputyHeadId).First();
            Models.User u = q;
            string name = u.Name;
            return name;
        }

        //public string getCurrentDepartmentHeadName(int currentDeputyHeadId)
        //{
        //    var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId).First();
        //    Models.User u = q;
        //    string name = u.Name;
        //    return name;
        //}

        public Models.DepartmentCoveringHeadRecord findCurrentDeputyHeadToEdit(int currentDeputyHeadId)
        {
            DateTime today = DateTime.Now.Date;
            var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId && x.EndDate >= today).First();
                DepartmentCoveringHeadRecord d2 = new DepartmentCoveringHeadRecord();
                d2 = q;
                return d2;
        }

        public List<User> populateAssignDeputyDropDownList(string dept, int repid, int headid)
        {
            var q = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid).OrderBy(x => x.Name).ToList();
            List<User> u = new List<User>();
            u = q;
            return u;
        }

        //ChangeCollectionPoint
        public Models.DisbursementList findDisbursementListStatus(string dept)
        {
            
            var q1 = context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Pending Delivery").First(); 
            DisbursementList d1 = new DisbursementList ();
            d1 = q1;
            return d1;
        }

        public Models.CollectionPoint findCollectionPointToEdit(int currentCollectionPoint)
        {
            
            var q2 = context.CollectionPoint.Where(x => x.CollectionPointId == currentCollectionPoint).First();
            CollectionPoint d2 = new CollectionPoint();
            d2 = q2;
            return d2;
        }

        //NewRequest 
        /*Generate RecoredID for EmployeeRequestRecord(depcode), PurchaseOrderRecord("PO"),
        AdjustmentRecord("V"), DisbursementList("DL") (e.g. XX201901150001)*/
        public string IDGenerator(string prefix)
        {
            DateTime today = DateTime.Today.Date;
            List<RecordDetails> rdList = context.RecordDetails.ToList();

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

        //OutstandingOrder
        public List<OutstandingOrder> getOutstandingOrders(string dept)
        {
            var q = from x in context.EmployeeRequestRecord
                    join s in context.User on x.DepEmpId equals s.UserId
                    where x.DepCode.Equals(dept) && x.Status == "Pending Approval"
                    select new OutstandingOrder
                    {
                        Rrid = x.Rrid,
                        Name = s.Name,
                        Status = x.Status
                    };
            return q.ToList();
        }

        public List<OutstandingOrderDetails> getOutstandingOrdersDetails(string rrid)
        {
            var q = from x in context.RecordDetails
                    join s in context.Catalogue on x.ItemNumber equals s.ItemNumber
                    where x.Rrid.Equals(rrid)
                    select new OutstandingOrderDetails
                    {
                        ItemName = s.ItemName,
                        Quantity = x.Quantity
                    };
            return q.ToList();
        }

        public Models.EmployeeRequestRecord findEmployeeRecord(String rrid)
        {

            var q2 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord d2 = new EmployeeRequestRecord();
            d2 = q2;
            return d2;
        }

        //ViewExpenditure
        public List<Renderview> GetExpenditureDetails(string Dlid)
        {
            var p = (from x in context.Catalogue
                     join b in context.RecordDetails on x.ItemNumber equals b.ItemNumber
                     join c in context.DisbursementList on b.Rrid equals c.Dlid
                     where c.Dlid.Equals(Dlid) && c.Status.Equals("Completed")

                     group new { x, b, c } by new { x.Category } into g

                     select new Renderview

                     {
                         Category = g.Key.Category,
                         Quantity = g.Sum(a => a.b.Quantity),
                         Subtotal = g.Sum(a => a.x.Supplier1Price * a.b.Quantity)

                     });


            return (p.ToList());
        }

        public List<DisbursementList> findDisbursementListStatusComplete(string dept)
        {
            var q1 = context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Completed").ToList();
            List<DisbursementList> dList = new List<DisbursementList>();
            dList = q1;
            return dList;
        }

        public List<DisbursementList> findDisbursementListStatusCompleteDateRange(string dept, DateTime startDate, DateTime endDate)
        {
            var q1 = context.DisbursementList.Where(s => s.StartDate >= startDate && s.CompleteDate <= endDate && s.Status == "Completed").ToList();
            List<DisbursementList> dList = new List<DisbursementList>();
            dList = q1;
            return dList;
        }

        public decimal findTotalExpenditure(String dept, List<DisbursementList> dList)
        {
            decimal sum = 0;

            List<RecordDetails> rList = new List<RecordDetails>();
            for (int i = 0; i < dList.Count; i++)
            {
                DisbursementList d = dList[i];
                string dId = d.Dlid;
                var q1 = context.RecordDetails.Where(x => x.Rrid == dId).ToList();
                foreach (RecordDetails current in q1)
                {
                    rList.Add(current);
                }
            }

            for (int j = 0; j < rList.Count; j++)
            {
                RecordDetails r = rList[j];
                string ItemNum = r.ItemNumber;
                var q2 = context.Catalogue.Where(x => x.ItemNumber == ItemNum).First();
                decimal price = (decimal)q2.Supplier1Price;
                int Quantity = r.Quantity;
                decimal total = price * Quantity;
                sum += total;
            }

            return sum;
        }


        public DisbursementList findDateRange(DateTime startDate, DateTime endDate)
        {
            var q = context.DisbursementList.Where(s => s.StartDate >= startDate && s.CompleteDate <= endDate && s.Status == "Completed").FirstOrDefault();
            DisbursementList d2 = new DisbursementList();
            d2 = q;
            return d2;
        }
        private void Save()
        {
            context.SaveChanges();
        }

        //TEST FOR ANDROID - Assign Department
        public List<String> getEmployeeNamesForAssignDept(string dept, int repid, int headid, int coveringheadid)
        {
            List<String> employeeNameList = new List<String>();
            var q = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid && x.UserId != coveringheadid).OrderBy(x => x.Name).ToList();
            List<User> userList = q;
            foreach (User current in userList)
            {
                string name = current.Name;
                employeeNameList.Add(name);
            }
            return employeeNameList;
        }
        //TEST FOR ANDROID - Assign Department
        public int getDeptHeadID(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int headid = q.HeadId;
            return headid;
        }
        public int getDeputyDeptHeadID(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int deputyheadid = q.HeadId;
            return deputyheadid;
        }
        public int getRepId(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int repid = q.RepId;
            return repid;
        }
        //TEST FOR ANDROID - Assign Department
        public int updateDepartment(string name, string dept)
        {
            var q = context.User.Where(x => x.Name == name).First();
            int userid = q.UserId;
            string department = q.DepartmentCode;
            var q1 = context.Department.Where(x => x.DepartmentCode == dept).First();
            Models.Department d = q1;
            d.RepId = userid;
            context.SaveChanges();
            return 1;
        }
        //TEST FOR ANDROID - Assign Deputy
        public List<String> populateAssignDeputy(string dept, int repid, int headid)
        {
            var q = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid).OrderBy(x => x.Name).ToList();
            List<User> u = new List<User>();
            u = q;
            List<String> list = new List<String>();
            foreach(User current in u)
            {
                list.Add(current.Name);
            }
            return list;
        }
        //TEST FOR ANDROID - Assign Deputy
        public void saveDeputy(string dept, string name, DateTime startDate, DateTime endDate)
        {
            Department department = context.Department.Where(x => x.DepartmentCode == dept).First();
            User u = context.User.Where(x => x.Name == name).First();
            Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
            d2.UserId = u.UserId;
            d2.StartDate = startDate;
            d2.EndDate = endDate;
            department.CoveringHeadId = u.UserId;
            context.Add(d2);
            context.SaveChanges();
        }
        public void updateDeputy(string dept, string name, DateTime startDate, DateTime endDate)
        {
            int currentDeputyId;
            Department department = context.Department.Where(x => x.DepartmentCode == dept).First();
             currentDeputyId = (int)department.CoveringHeadId;
                var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyId).First();
            Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
            d2 = q;
            User u = context.User.Where(x => x.Name == name).First();
            d2.UserId = u.UserId;
            d2.StartDate = startDate;
            d2.EndDate = endDate;

            department.CoveringHeadId = u.UserId;
            context.SaveChanges();
        }

    }
}
