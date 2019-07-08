using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("comments")]
    public class Comment
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int comment_id { get; set; }
        public int order_id { get; set; }
        public float grade { get; set; }
        public string comment_text { get; set; }
        public string times { get; set; }
        public string user_id { get; set; }
    }
}