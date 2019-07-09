using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("activity_instance")]
    public class ActivityInstance
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int activity_instance_id { get; set; }
        public int activity_id { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public double price { get; set; }
        public int is_booked { get; set; }
    }
}