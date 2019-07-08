using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("activity")]
    public class Activity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int activity_id { get; set; }
        public string activity_name { get; set; }
        public string user_id { get; set; }
        public string address { get; set; }
        public string act_content { get; set; }
        public int max_member_limit { get; set; }
        public string activity_pictures { get; set; }
        public string note { get; set; }
        public string cancellation_policy { get; set; }
    }
}