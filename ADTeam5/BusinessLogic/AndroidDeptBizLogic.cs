
using ADTeam5.Models;
using ADTeam5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADTeam5.BusinessLogic
{
    public class AndroidDeptBizLogic
    {
        private readonly SSISTeam5Context context = new SSISTeam5Context();

        //List employees
        public List<BriefEmp> ListEmployees(string dept)
        {
            var t = context.Department.Where(x => x.DepartmentCode == dept).First();
            int headid = t.HeadId;
            int repid = t.RepId;

            var q = from x in context.User
                    where x.DepartmentCode == dept && x.UserId != headid && x.UserId != repid
                    select new BriefEmp
                    {
                        userId = x.UserId,
                        name = x.Name
                    };

            return q.ToList();
        }

        //Get current dept rep
        public BriefEmp GetRep(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int repid = q.RepId;
            var p = context.User.Where(x => x.UserId == repid).First();
            BriefEmp b = new BriefEmp();
            b.userId = p.UserId;
            b.name = p.Name;
            return b;
        }

        //Save Changes for Department Rep
        public void SaveNewDeptRep(BriefDept briefDept)
        {
            string dept = briefDept.deptCode;

            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            Department d = q;
            d.RepId = briefDept.repId;
            context.SaveChanges();
        }

        //Get Outstanding Orders
        public List<BriefEmpReq> getOutstandingOrders(string dept)
        {
            List<BriefEmpReq> list = new List<BriefEmpReq>();

            var q = from x in context.EmployeeRequestRecord
                    join s in context.User on x.DepEmpId equals s.UserId
                    where x.DepCode == dept && x.Status == "Pending Approval"
                    orderby x.RequestDate
                    select new BriefEmpReq
                    {
                        rrid = x.Rrid,
                        requestDate = x.RequestDate,
                        userid = x.DepEmpId,
                        name = s.Name,
                        status = x.Status
                    };

            list = q.ToList();
            return list;
        }

        public List<BriefRecDetails> getRecordDetails(string rrid)
        {
            List<BriefRecDetails> list = new List<BriefRecDetails>();

            var q = from x in context.RecordDetails
                    join s in context.Catalogue on x.ItemNumber equals s.ItemNumber
                    where x.Rrid == rrid
                    select new BriefRecDetails
                    {
                        rrid = x.Rrid,
                        itemNum = x.ItemNumber,
                        itemName = s.ItemName,
                        quantity = x.Quantity
                    };
            list = q.ToList();
            return list;
        }

        //Approve Outstanding Orders
        public void ApproveRequest(BriefEmpReq briefEmpReq)
        {
            string rrid = briefEmpReq.rrid;

            var q = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e = q;
            e.Status = "Approved";
            context.SaveChanges();
        }

        //Reject Outstanding Orders
        public void RejectRequest(BriefEmpReq briefEmpReq)
        {
            string rrid = briefEmpReq.rrid;
            string remark = briefEmpReq.remark;

            var q = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e = q;
            e.Status = "Rejected";
            e.Remark = remark;
            context.SaveChanges();
        }

        //Generate random number for collection point
        public BriefDept generateCollectionPassword(string dept)
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            int num = _rdm.Next(_min, _max);
            string randomNumber = num.ToString();
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int repId = q.RepId;
            Department d = new Department();
            d = q;
            d.CollectionPassword = num.ToString();
            context.SaveChanges();
            BriefDept briefDept = new BriefDept();
            briefDept.deptCode = dept;
            briefDept.collectionPassword = randomNumber;
            briefDept.repId = repId;
            return briefDept;
        }

        //Get current collection point
        public BriefDept2 currentCollectionPoint(string dept)
        {
            var q = context.Department.Where(x => x.DepartmentCode == dept).First();
            int collectionPointId = q.CollectionPointId;
            BriefDept2 d = new BriefDept2();
            d.deptCode = dept;
            d.collectionPointId = collectionPointId;
            var t = context.CollectionPoint.Where(x => x.CollectionPointId == collectionPointId).First();
            string collectionPointName = t.CollectionPointName;
            d.collectionPointName = collectionPointName;

            return d;
        }

        //Change collection point
        public void changeCollectionPoint(BriefDept2 briefDept2)
        {
            string deptCode = briefDept2.deptCode;
            string collectionPointName = briefDept2.collectionPointName;

            var p = context.CollectionPoint.Where(x => x.CollectionPointName == collectionPointName).First();
            int newCollectionPointId = p.CollectionPointId;

            var q = context.Department.Where(x => x.DepartmentCode == deptCode).First();
            Department d = new Department();
            d = q;
            d.CollectionPointId = newCollectionPointId;

            TimeSpan cutoff = TimeSpan.Parse("17:30"); // 5.30 PM
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || (DateTime.Now.DayOfWeek == DayOfWeek.Friday && now <= cutoff))
            {
                var t = context.DisbursementList.Where(x => x.DepartmentCode == deptCode && x.Status == "Pending Delivery");
                if (t.Any())
                {
                    DisbursementList dl = new DisbursementList();
                    dl = t.First();
                    dl.CollectionPointId = newCollectionPointId;
                }
            }
            context.SaveChanges();
        }

        //List collection points
        public List<BriefDept2> listCollectionPoints()
        {
            var q = context.CollectionPoint.ToList();
            List<BriefDept2> list = new List<BriefDept2>();
            foreach (CollectionPoint current in q)
            {
                BriefDept2 b = new BriefDept2();
                b.collectionPointId = current.CollectionPointId;
                b.collectionPointName = current.CollectionPointName;
                list.Add(b);
            }
            return list;
        }

        //Get upcoming disbursement
        public BriefDisbursement getBriefDisbursement(string dept)
        {
            var t = context.DisbursementList.Where(s => s.DepartmentCode == dept && s.Status == "Pending Delivery");

            if (t.Any())
            {
                var q = from x in context.DisbursementList
                        join p in context.CollectionPoint on x.CollectionPointId equals p.CollectionPointId
                        where x.DepartmentCode == dept && x.Status == "Pending Delivery"
                        select new BriefDisbursement
                        {
                            collectionDate = x.EstDeliverDate,
                            collectionPointName = p.CollectionPointName,
                            collectionTime = p.CollectionTime
                        };
                return q.First();
            }

            return null;

        }


    }
}