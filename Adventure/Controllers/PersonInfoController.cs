using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Adventure.Models;
using SqlSugar;
using DbType = SqlSugar.DbType;
using System.Security.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adventure.Controllers
{
    public class PersonInfoController : Controller
    {
        // GET: PersonInfo
        public ActionResult Index()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/Login/index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            DateTime dt = DateTime.Now;
            string folder = Server.MapPath("~/images/headicon");
            var filePath = folder + "\\" + dt.ToFileTime().ToString()+ Request.Files["filedata"].FileName.Substring(Request.Files["filedata"].FileName.LastIndexOf("\\") + 1);
            var fileSave= Request.Files["filedata"].FileName; 
            if (Request.Files["filedata"].FileName!="")
            {
                fileSave = "../images/headicon/" + dt.ToFileTime().ToString() + Request.Files["filedata"].FileName.Substring(Request.Files["filedata"].FileName.LastIndexOf("\\") + 1);

            }else{
                fileSave = "../iamges/headicon/default.jpg";
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Request.Files["filedata"].SaveAs(filePath);
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                });

            
            try
            {

                var data = new User()
                {
                    user_id = (string)Session["form-username"],
                    head_icon = fileSave,
                    first_name = Request.Form["form-first-name"],
                    last_name = Request.Form["form-last-name"],
                    pass_word = GetMD5(Request.Form["form-password"]),
                    gender = Convert.ToInt16(Request.Form["form-gender"]),
                    email_address = Request.Form["form-email"],
                    phone_number = Request.Form["form-phoneNumber"],
                    main_language = Request.Form["form-language"],
                    country = Request.Form["form-country"],
                    self_introduction = Request.Form["form-about-yourself"],
                    bonus_points = (int)Session["bonus_points"]
                };
                //db.Updateable(data).IgnoreColumns(it=>new {it.user_id, it.bonus_points}).ExecuteCommand()== 1
                if (db.Updateable(data).ExecuteCommand()==1)
                {
                    var getUserbyUserID = db.Queryable<User>().Where(it => it.user_id == Request.Form["form-username"]).ToList();
                    ViewBag.errorMessage = "更新成功！";
                    ViewBag.flag = 1;
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
                    return View( );
                }
                else
                {
                    ViewBag.errorMessage = "更新失败!";
                    ViewBag.flag = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "更新失败!";
                ViewBag.flag = 0;
                throw ex;
            }
            finally { }
            //return View("Index");
            
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