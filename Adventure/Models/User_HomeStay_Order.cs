using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("user_homestay_order")]
    public class User_Homestay_Order
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int user_id { get; set; }
        public int homestay_id { get; set; }
        public int order_id { get; set; }
    }
}