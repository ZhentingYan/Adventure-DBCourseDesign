using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("report")]
    public class Report
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int report_id { get; set; }
        public string user_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int order_id { get; set; }
        public string reason { get; set; }
        public DateTime times { get; set; }
    }
}