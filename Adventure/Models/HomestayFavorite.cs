using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("homestay_favorite")]
    public class HomestayFavorite
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public int homestay_id { get; set; }
        public DateTime times { get; set; }
    }
}