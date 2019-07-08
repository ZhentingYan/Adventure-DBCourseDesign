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

namespace Adventure.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                });
            JObject isSuccess = new JObject();
            try
            {
                var data = new User()
                {
                    user_id = form["form-username"],
                    head_icon = "../images/headicon/default.jpg",
                    first_name = form["form-first-name"],
                    last_name = form["form-last-name"],
                    pass_word = GetMD5(form["form-password"]),
                    gender = Convert.ToInt16(form["form-gender"]),
                    email_address = form["form-email"],
                    phone_number = form["form-phoneNumber"],
                    main_language = form["form-language"],
                    country = form["form-country"],
                    self_introduction = form["form-about-yourself"],
                    bonus_points = 0
                };

                if (db.Insertable(data).ExecuteCommand() == 1)
                {
                    ViewBag.errorMessage = "注册成功,欢迎探索Adventure!";
                    ViewBag.flag = 1;
                    return View();
                }
                else
                {
                    ViewBag.errorMessage = "注册失败!";
                    ViewBag.flag = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "注册失败!";
                ViewBag.flag = 0;
                return View();

            }
            finally { }

            //return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

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
            var a = 0;
            return ret;
        }
    }
}