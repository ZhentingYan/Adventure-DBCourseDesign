using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SqlSugar;



namespace Adventure.Models
{
    [SugarTable("users")]
    public class User
    {
        public string user_id { get; set; }
        public string head_icon { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string pass_word { get; set; }
        public int gender { get; set; }
        public string email_address { get; set; }
        public string phone_number { get; set; }
        public string main_language { get; set; }
        public string country { get; set; }
        public string self_introduction { get; set; }
        public int bonus_points { get; set; }
    }
}