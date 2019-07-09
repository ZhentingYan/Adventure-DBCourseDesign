using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("homestay_comment")]
    public class HomestayComment
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int homestay_comment_id { get; set; }
        public int homestay_order_id { get; set; }
        public double grade { get; set; }
        public string user_id { get; set; }
        public string comment_text { get; set; }
        public DateTime times { get; set; }
    }
}