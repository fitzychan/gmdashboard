using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataModels
{
    public class InternalTracker
    {
        public bool allow_only_contributors_to_track_time { get; set; }
        public bool enable_issue_dependencies { get; set; }
        public bool enable_time_tracker { get; set; }
    }
}
