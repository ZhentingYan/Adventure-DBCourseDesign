using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace Adventure.Models
{
    [SugarTable("activity_favorite")]
    public class ActivityFavorite
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public int activity_id { get; set; }
        public DateTime times { get; set; }

    }
}