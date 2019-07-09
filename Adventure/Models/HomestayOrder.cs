using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("homestay_order")]
    public class HomestayOrder
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int homestay_order_id { get; set; }
        public int homestay_id { get; set; }
        public int status { get; set; }
        public DateTime create_time { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public double total_price { get; set; }
        public double platform_fee { get; set; }
        public string payment_method { get; set; }
    }
}