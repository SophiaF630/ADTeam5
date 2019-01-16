using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Controllers.Department
{
    public class NewRequest
    {
        string rrid;
        DateTime requestDate;
        int depHeadId;
        int depEmpId;
        string depCode;
        string status;

        public NewRequest()
        {

        }
        public NewRequest(string rrid, int depHeadId, int depEmpId, string depCode)
        {
            this.rrid = rrid;
            this.requestDate = DateTime.Now;
            this.depHeadId = depHeadId;
            this.depEmpId = depEmpId;
            this.depCode = depCode;
            this.status = "Submitted";
        }
        public string Rrid
        {
            get { return rrid; }
            set { rrid = value; }
        }
        public DateTime RequestDate
        {
            get { return requestDate; }
            set { requestDate = value; }
        }
        public int DepHeadId
        {
            get { return depHeadId; }
            set { depHeadId = value; }
        }
        public int DepEmpId
        {
            get { return depEmpId; }
            set { depEmpId = value; }
        }
        public string DepCode
        {
            get { return depCode; }
            set { depCode = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
