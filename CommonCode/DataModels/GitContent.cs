using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataModels
{
    public class GitContent
    {
        public string name { get; set; }
        public string path { get; set; }
        public string sha { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string encoding { get; set; }
        public string content { get; set; }
        public IList<string> target { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string git_url { get; set; }
        public string download_url { get; set; }
        public IList<string> submodule_git_url { get; set; }
        public Links links { get; set; }

    }
}
