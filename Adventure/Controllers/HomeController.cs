using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Newtonsoft.Json;


namespace Adventure.Controllers
{

    public class HomeController : Controller
    {
        public class JsonMessage
        {
            //状态
            private bool _statues;

            public bool status
            {
                get { return _statues; }
                set { _statues = value; }
            }
            //消息
            private string _msg;

            public string msg
            {
                get { return _msg; }
                set { _msg = value; }
            }
            //数据
            private object _data;
            public object data
            {
                get { return _data; }
                set { _data = value; }
            }
        }
    
        [HttpGet]
        public JsonResult getCount()
        {
            String connection = "User Id=system;Password=tkh603;Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))" + "(CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = orcl)))"; ;
            decimal customerCount,homestayCount, activityCount, reviewCount;
            using (OracleConnection conn = new OracleConnection(connection))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("select count(*) from users", conn);//执行一条SQL语句
                customerCount = (decimal)cmd.ExecuteScalar();
                 cmd = new OracleCommand("select count(*) from Homestay", conn);//执行一条SQL语句
                homestayCount = (decimal)cmd.ExecuteScalar();
                 cmd = new OracleCommand("select count(*) from Activity", conn);//执行一条SQL语句
                activityCount = (decimal)cmd.ExecuteScalar();
                 cmd = new OracleCommand("select count(*) from Comments", conn);//执行一条SQL语句
                reviewCount = (decimal)cmd.ExecuteScalar();
                conn.Close();
            }
            JsonMessage m = new JsonMessage();
            var jsonData = "{ \"customerCount\":\""+customerCount.ToString()+"\", \"homestayCount\":\""+homestayCount.ToString()+"\", \"activityCount\":\"" + activityCount.ToString() + "\", \"reviewCount\":\"" + reviewCount.ToString()+"\"}";
            m.status = true;
            m.msg = "return Success";
            m.data = jsonData;
            return Json(m, JsonRequestBehavior.AllowGet); 
        }
        public ActionResult Index()
        {
            /*
            if (Session["user"] == null)
                return Redirect("~/Login/index");
            else
            {
                ViewBag.title = "度假屋、民宿、体验与旅行故事";
                return View();
            }
            */
            ViewBag.title = "度假屋、民宿、体验与旅行故事";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}