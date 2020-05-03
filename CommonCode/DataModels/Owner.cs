using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataModels
{
    public class Owner
    {
        public string avatar_url { get; set; }
        public DateTime created { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public int id { get; set; }
        public bool is_admin { get; set; }
        public string language { get; set; }
        public DateTime last_login { get; set; }
        public string login { get; set; }
    }
}
