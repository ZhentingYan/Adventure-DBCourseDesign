using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;


namespace Adventure.Models
{
    [SugarTable("homestay")]
    public class Homestay
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int homestay_id { get; set; }
        public string user_id { get; set; }
        public string homestay_name { get; set; }
        public string homestay_pictures { get; set; }
        public string homestay_type { get; set; }
        public int num_of_bedrooms { get; set; }
        public int num_of_beds { get; set; }
        public int num_of_bathrooms { get; set; }
        public int max_member_limit { get; set; }
        public string introduction { get; set; }
        public int default_price { get; set; }
        public string check_in_method { get; set; }
        public double house_grade { get; set; }
        public string convenience_facilities { get; set; }
        public string latest_schedulable_date { get; set; }
        public string address { get; set; }
        public string house_regulations { get; set; }
        public string cancellation_policy { get; set; }
    }
}