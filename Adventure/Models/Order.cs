using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("orders")]
    public class Order
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int order_id { get; set; }
        public string status { get; set; }
        public string create_time { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public int total_price { get; set; }
        public int platform_fee { get; set; }
        public string payment_method { get; set; }
    }
}