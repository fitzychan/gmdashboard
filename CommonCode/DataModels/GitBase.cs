using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataModels
{
    public class GitBase
    {
        public bool allow_merge_commits { get; set; }
        public bool allow_rebase { get; set; }
        public bool allow_rebase_explicit { get; set; }
        public bool allow_squash_merge { get; set; }
        public bool archived { get; set; }
        public string avatar_url { get; set; }
        public string clone_url { get; set; }
        public DateTime created_at { get; set; }
        public string default_branch { get; set; }
        public string description { get; set; }
        public bool empty { get; set; }
        public ExternalTracker external_tracker { get; set; }
        public ExternalWiki external_wiki { get; set; }
        public bool fork { get; set; }
        public int forks_count { get; set; }
        public string full_name { get; set; }
        public bool has_issues { get; set; }
        public bool has_pull_requests { get; set; }
        public bool has_wiki { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public bool ignore_whitespace_conflicts { get; set; }
        public InternalTracker internal_tracker { get; set; }
        public bool mirror { get; set; }
        public string name { get; set; }
        public int open_issues_count { get; set; }
        public int open_pr_counter { get; set; }
        public string original_url { get; set; }
        public Owner owner { get; set; }
        public Permissions permissions { get; set; }
        public bool @private { get; set; }
        public int release_counter { get; set; }
        public int size { get; set; }
        public string ssh_url { get; set; }
        public int stars_count { get; set; }
        public bool template { get; set; }
        public DateTime updated_at { get; set; }
        public int watchers_count { get; set; }
        public string website { get; set; }
    }
}
