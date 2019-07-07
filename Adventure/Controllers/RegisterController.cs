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
        public ActionResult AddUser(FormCollection form)
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = "User Id=system;Password=Homestay123;Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orcl)))",
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                });
            JObject isSuccess = new JObject();
            try
            {
                var data = new User()
                {
                    user_id = "Hello",
                    head_icon = "../images/headicon/default.png",
                    first_name = form["form-first-name"],
                    last_name = form["form-last-name"],
                    pass_word = form["form-password"],
                    gender = Convert.ToInt16(form["form-gender"]),
                    email_address = form["form-email"],
                    phone_number = form["form-phoneNumber"],
                    main_language = form["form-language"],
                    country = form["form-country"],
                    self_introduction = form["form-about-yourself"],
                    bonus_points = 100
                };

                if (db.Insertable(data).ExecuteCommand() == 1)
                {
                    isSuccess.Add("isAdd", "Y");
                }
                else
                {
                    isSuccess.Add("isAdd", "N");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }

            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

        }
    }
}