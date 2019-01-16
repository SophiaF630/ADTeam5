using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Models
{
    interface IGenerate
    {
         int IDGenerator(string prefix);
    }
}
