using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("activity_instance")]
    public class Activity_Instance
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int user_id { get; set; }
        public int activity_id { get; set; }
        public int order_id { get; set; }
    }
}