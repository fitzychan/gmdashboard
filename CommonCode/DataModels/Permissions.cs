using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataModels
{
    public class Permissions
    {
        public bool admin { get; set; }
        public bool pull { get; set; }
        public bool push { get; set; }
    }
}
