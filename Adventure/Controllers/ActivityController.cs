﻿using System;
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
    public class ActivityController : Controller
    {
        public ActionResult EditActivity(int productID=-1)
        {
            ViewBag.productID = productID;
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
                var getActivitySpecific = db.Queryable<Activity>().InSingle(productID);
                ViewBag.ActivityDetail = getActivitySpecific;
                Session["activity_pic"] = getActivitySpecific.activity_pictures;
                var Publisher = db.Queryable<User>().InSingle(getActivitySpecific.user_id);
                ViewBag.Publisher = Publisher;
                if (getActivitySpecific.user_id!=(string)Session["user_id"])
                    return Redirect("~/Home");

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "读取活动信息失败!";
                ViewBag.flag = 0;
                return Redirect("~/Home");
            }
            finally { }
            return View();
        }
        [HttpPost]
        public ActionResult submitEdit(FormCollection form)
        {
            DateTime dt = DateTime.Now;
            string folder = Server.MapPath("~/images/Activity");
            var filePath = folder + "\\" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);
            var fileSave = Request.Files["myfile"].FileName;
            if (Request.Files["myfile"].FileName != "")
            {
                fileSave = "../images/Activity/" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);

            }
            else
            {
                fileSave = (string)Session["activity_pic"];
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Request.Files["myfile"].SaveAs(filePath);
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
                var data = new Activity()
                {
                    activity_id= Convert.ToInt16(Request.Form["activity_ID"]),
                    address = Request.Form["activity_place"],
                    user_id = (string)Session["user_id"],
                    activity_name = Request.Form["activity_title"],
                    max_member_limit = Convert.ToInt16(Request.Form["activity_capacity"]),
                    act_content = Request.Form["activity_content"],
                    activity_pictures = fileSave,
                    note = Request.Form["activity_note"],
                    cancellation_policy = Request.Form["activity_cancel"]

                };

                if (db.Updateable(data).ExecuteCommand() == 1)
                {
                    int productID = db.Queryable<Activity>().Max(it => it.activity_id);
                    ViewBag.errorMessage = "活动更新成功!活动ID:" + productID;
                    ViewBag.flag = 1;
                    ViewBag.productID = productID;
                    return Redirect("~/Activity/SingleActivity?productID=" + Request.Form["activity_ID"]);
                }
                else
                {
                    ViewBag.errorMessage = "活动更新失败";
                    ViewBag.flag = 0;
                    return Redirect("~/Activity/EditActivity?productID=" + Request.Form["activity_ID"]);
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "活动更新失败";
                ViewBag.flag = 0;
                return Redirect("~/Activity/EditActivity?productID=" + Request.Form["activity_ID"]);

            }
            finally { }
        }


        // GET: Activity
        public ActionResult Index()
        {

            return View();
        }

        // GET: Single Activity
        public ActionResult SingleActivity(int productID=-1)
        {
            ViewBag.productID = productID;
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
                var getActivitySpecific = db.Queryable<Activity>().InSingle(productID);
                ViewBag.ActivityDetail = getActivitySpecific;
                ViewBag.Publisher = db.Queryable<User>().InSingle(getActivitySpecific.user_id);
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "活动发布失败";
                ViewBag.flag = 0;
                return Redirect("~/Activity/Publish");
            }
            finally { }
            List<int> list = null;
            if (Session["track"] == null)
            {
                list = new List<int>();
            }
            else
            {
                list = (List<int>)Session["track"];
            }

            if (list.Contains(productID))
                list.Remove(productID);    // 如果这次浏览的商品在浏览记录中,删除后重新添加进去,保持浏览顺序
            if (list.Count == 10)
                list.RemoveAt(0);    // 浏览记录保存10个,到了10个删除最老的一个记录.
            list.Add(productID);           // 浏览顺序为list[9] list[8].....list[1] list[0]
            Session["track"] = list;    // 把list更新到Session 
            return View();
        }

        // GET: Publish Activity
        public ActionResult Publish()
        {
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
                    var myActivityList=db.Queryable<Activity>().Where(it => it.user_id==Session["user_id"]).ToArray();
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
       
        [HttpPost]
        public ActionResult Publish(FormCollection form)
        {
            //var address = Request.Form["activity_place"];
            DateTime dt = DateTime.Now;
            string folder = Server.MapPath("~/images/Activity");
            var filePath = folder + "\\" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);
            var fileSave = Request.Files["myfile"].FileName;
            if (Request.Files["myfile"].FileName != "")
            {
                fileSave = "../images/Activity/" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);

            }
            else
            {
                fileSave = "../iamges/Activity/default.jpg";
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Request.Files["myfile"].SaveAs(filePath);
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
                var data = new Activity()
                {
                    address = Request.Form["activity_place"],
                    user_id = (string)Session["user_id"],
                    activity_name = Request.Form["activity_title"],
                    max_member_limit = Convert.ToInt16(Request.Form["activity_capacity"]),
                    act_content = Request.Form["activity_content"],
                    activity_pictures = fileSave,
                    note = Request.Form["activity_note"],
                    cancellation_policy = Request.Form["activity_cancel"]

                };
                if (db.Insertable(data).ExecuteCommand() >= 1)
                {
                    int productID = db.Queryable<Activity>().Max(it => it.activity_id);
                    ViewBag.errorMessage = "活动发布成功!活动ID:"+productID;
                    ViewBag.flag = 1;
                    ViewBag.productID = productID;
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
                    return View();
                }
                else
                {
                    ViewBag.errorMessage = "活动发布失败";
                    ViewBag.flag = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "活动发布失败";
                ViewBag.flag = 0;
                return View();

            }
            finally { }
        }
    }
}