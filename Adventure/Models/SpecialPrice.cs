using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("special_price")]
    public class SpecialPrice
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int special_price_id { get; set; }
        public int homestay_id { get; set; }
        public DateTime beginning_date { get; set; }
        public DateTime end_date { get; set; }
        public double price { get; set; }
    }
}