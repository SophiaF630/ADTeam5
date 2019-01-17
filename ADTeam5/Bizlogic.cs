using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
namespace ADTeam5
{
    public class Bizlogic
    {
        SSISTeam5Context m = new SSISTeam5Context();

        public List<String> GetStaffName
        {
            get
            {
                return m.User.Select<User, string>(c => c.Name).ToList<String>();

            }
        }

        public List<String> GetItemDescription
        {
            get
            {
                return m.Catalogue.Select<Catalogue, string>(c => c.ItemName).ToList<String>();
            }
        }
    }
}
