using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using Adventure.Models;
using SqlSugar;
using DbType = SqlSugar.DbType;
using System.Security.Authentication;

namespace Adventure.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (Session["user_id"] != null)
                Session.Abandon();


            return View();
        }
        [HttpPost]
        public ActionResult getUser()
        {

            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = "User Id=system;Password=tkh603;Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orcl)))",
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });
            JObject isSuccess = new JObject();
            try
            {
                var getUserbyUserID = db.Queryable<User>().Where(it=>it.user_id==Request.Form["userID"]).ToList();

                if (getUserbyUserID[0].pass_word == GetMD5(Request.Form["userPwd"]))
                {
                    isSuccess.Add("isLogin", true);
                    Session["user_id"] = getUserbyUserID[0].user_id;
                    Session["head_icon"] = getUserbyUserID[0].head_icon;
                    Session["first_name"] = getUserbyUserID[0].first_name;
                    Session["last_name"] = getUserbyUserID[0].last_name;
                    Session["pass_word"] = getUserbyUserID[0].pass_word;
                    Session["gender"] = getUserbyUserID[0].gender;
                    Session["email_address"] = getUserbyUserID[0].email_address;
                    Session["phone_number"] = getUserbyUserID[0].phone_number;
                    Session["main_language"] = getUserbyUserID[0].main_language;
                    Session["country"] = getUserbyUserID[0].country;
                    Session["self_introduction"] = getUserbyUserID[0].self_introduction;
                    Session["bonus_points"] = getUserbyUserID[0].bonus_points;
                }
                else
                {
                    isSuccess.Add("isLogin", false);
                }
            }
            catch(Exception ex)
            {
                isSuccess.Add("isLogin", false);
            }
            finally { }
            
           

            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

        }
        public static string GetMD5(string str)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = " ";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
    }
}