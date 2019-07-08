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
    public class AdminController : Controller
    {
        // GET: Admin
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
        public ActionResult ViewHomestayOrder()
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
        public ActionResult EditHomestayOrder()
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
        public ActionResult ViewActivityOrder()
        {
            ViewBag.Message = "Your ActivityOrder page.";
            if (Session["user_id"] == null)
            {
                return Redirect("~/Login/index");
            }
            else
            {
                SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                DbType = DbType.Oracle,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
                JObject isSuccess = new JObject();
                try
                {
                    var myActivityList = db.Queryable<Activity>().Where(it => it.user_id == Session["user_id"]).ToArray();
                    ViewBag.activityList = myActivityList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                ViewBag.Message = "Publish your own activity page.";
                return View();
            }
        }
        public ActionResult EditActivityOrder()
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
        public ActionResult AddHomestay()
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
        public ActionResult ViewHomestay()
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
        public ActionResult EditHomestay()
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
        public ActionResult AddActivity()
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
        public ActionResult ViewActivity()
        {
            ViewBag.Message = "Your ActivityOrder page.";
            if (Session["user_id"] == null)
            {
                return Redirect("~/Login/index");
            }
            else
            {
                SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                DbType = DbType.Oracle,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
                JObject isSuccess = new JObject();
                try
                {
                    var myActivityList = db.Queryable<Activity>().Where(it => it.user_id == Session["user_id"]).ToArray();
                    ViewBag.activityList = myActivityList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                ViewBag.Message = "Publish your own activity page.";
                return View();
            }
        }
        public ActionResult EditActivity()
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
        public ActionResult Compensation()
        {
            ViewBag.Message = "Your Compensation page.";

            if (Session["user_id"] == null)
            {
                return Redirect("~/Login/index");
            }
            else
            {
                return View();
            }
        }
    }
}