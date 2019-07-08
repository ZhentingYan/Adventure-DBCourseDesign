using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace Adventure.Models
{
    [SugarTable("story")]
    public class Story
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsOnlyIgnoreInsert = true)]
        public int story_id { get; set; }
        public string user_id { get; set; }
        public string title { get; set; }
        public string content_s { get; set; }
        public string pictures { get; set; }
        public string times { get; set; }

    }
}