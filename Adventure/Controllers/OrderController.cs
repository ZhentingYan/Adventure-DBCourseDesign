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
using System.Globalization;

namespace Adventure.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order

        [HttpGet]
        public ActionResult Index(int productID = -1, int instanceID = -1)
        {
            if (Session["user_id"] == null)

            {
                Session["message"] = "你还没有登陆哦！";
                return Redirect("~/Login/index");
            }
            else
            {
                if (productID == -1 || instanceID == -1)
                {
                    Session["message"] = "非法访问!";
                    return Redirect("~/Home");
                }
                //先这样，做不下去了，乱七八糟
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
                    ViewBag.productID = productID;
                    ViewBag.instanceID = instanceID;
                    var activityInstance = db.Queryable<ActivityInstance>().InSingle(instanceID);
                    var getActivitySpecific = db.Queryable<Activity>().InSingle(productID);
                    if (activityInstance != null && getActivitySpecific != null)
                    {
                        ViewBag.activityInstance = activityInstance;
                        ViewBag.activity = getActivitySpecific;
                        return View();

                    }
                    else
                    {
                        Session["message"] = "获取订单详情异常!";
                        return Redirect("~/Home");
                    }
                }
                catch (Exception ex)
                {
                    Session["message"] = "获取订单详情异常!";
                    return Redirect("~/Home");
                }
            }

        }
        [HttpPost]
        public ActionResult AddActivityOrder()
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
                DateTime dt_start_time, dt_end_time;
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy/MM/dd";
                dt_start_time = Convert.ToDateTime(Request.Form["start_time"], dtFormat);
                dt_end_time = Convert.ToDateTime(Request.Form["end_time"], dtFormat);
                DateTime dt_create = DateTime.Now;
                double p = Convert.ToDouble(Request.Form["price"]);
                double fee = p * 5 / 100;

                var dataOrder = new ActivityOrder()
                {
                    status = -1,
                    customer_id = Session["user_id"].ToString(),
                    activity_instance_id = Convert.ToInt32(Request.Form["activity_instance_id"]),
                    create_time = dt_create,
                    start_time = dt_start_time,
                    end_time = dt_end_time,
                    total_price = p + fee,
                    platform_fee = fee,
                    payment_method = Request.Form["payment"]
                };
                if (db.Insertable(dataOrder).ExecuteCommand() == 1)
                {

                    if (db.Updateable<ActivityInstance>().UpdateColumns(it => new ActivityInstance() { is_booked = 1 })
                        .Where(it => it.activity_instance_id == Convert.ToInt32(Request.Form["activity_instance_id"])).ExecuteCommand() == 1)
                    {
                        isSuccess.Add("isSuccess", true);
                    }
                    else
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "实例访问失败，请重试！");
                    }
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "订单创建失败，请重试！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败！");
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }
        public ActionResult HomestayOrder(int productID = -1, int personNum = -1, string start_time = "", string end_time = "")
        {
            if (productID == -1|| personNum == -1 || start_time == "" || end_time == "")
            {
                Session["message"] = "参数非法！";
                return Redirect("~/Login/index");
            }

            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });
            try
            {
                if (Session["user_id"] == null)
                {
                    return Redirect("~/Login/index");
                }
                DateTime dt_start_time, dt_end_time;
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy/MM/dd";
                dt_start_time = Convert.ToDateTime(start_time, dtFormat);
                dt_end_time = Convert.ToDateTime(end_time, dtFormat);
                TimeSpan span = dt_end_time.Subtract(dt_start_time);
                int dayDiff = span.Days;

                TimeSpan temp;
                int timespace;

                double p = 0;
                var specialPrice = db.Queryable<SpecialPrice>().Where(it => it.homestay_id == productID && dt_start_time <= it.end_date && dt_end_time > it.beginning_date).ToArray();
                for (int i = 0; i < specialPrice.Length; i++)
                {
                    if (specialPrice[i].beginning_date > dt_start_time && specialPrice[i].end_date >= dt_end_time)
                    {
                        temp = dt_end_time.Subtract(specialPrice[i].beginning_date);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else if (specialPrice[i].beginning_date > dt_start_time && specialPrice[i].end_date < dt_end_time)
                    {
                        temp = specialPrice[i].beginning_date.Subtract(specialPrice[i].end_date);
                        timespace = temp.Days + 1;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else if (specialPrice[i].beginning_date <= dt_start_time && specialPrice[i].end_date <= dt_end_time)
                    {
                        temp = specialPrice[i].end_date.Subtract(dt_start_time);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else
                    {
                        temp = dt_end_time.Subtract(dt_start_time);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                }
                var Homestay = db.Queryable<Homestay>().InSingle(productID);
                p = p + dayDiff * Homestay.default_price;
                double fee = p * 5 / 100;
                ViewBag.homestay = Homestay;
                ViewBag.total_price = p;
                ViewBag.platform_fee = fee;
                ViewBag.start_time = start_time;
                ViewBag.end_time = end_time;
                ViewBag.person_num = personNum;

            }
            catch (Exception ex)
            {
                Session["message"] = "操作失败！";
                return Redirect("~/Home");
            }
            return View();
        }


        [HttpPost]
        public ActionResult AddHomeStayOrder()
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
                DateTime dt_start_time, dt_end_time;
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy/MM/dd";
                dt_start_time = Convert.ToDateTime(Request.Form["start_time"], dtFormat);
                dt_end_time = Convert.ToDateTime(Request.Form["end_time"], dtFormat);
                DateTime dt_create = DateTime.Now;
                TimeSpan span = dt_end_time.Subtract(dt_start_time);
                int dayDiff = span.Days;

                TimeSpan temp;
                int timespace;
                /*
                double p = 0;
                var specialPrice = db.Queryable<SpecialPrice>().Where(it => it.homestay_id == Convert.ToInt32(Request.Form["homestay_id"]) && dt_start_time <= it.end_date && dt_end_time > it.beginning_date).ToArray();

                for (int i = 0; i < specialPrice.Length; i++)
                {
                    if (specialPrice[i].beginning_date > dt_start_time && specialPrice[i].end_date >= dt_end_time)
                    {
                        temp = dt_end_time.Subtract(specialPrice[i].beginning_date);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else if (specialPrice[i].beginning_date > dt_start_time && specialPrice[i].end_date < dt_end_time)
                    {
                        temp = specialPrice[i].beginning_date.Subtract(specialPrice[i].end_date);
                        timespace = temp.Days + 1;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else if (specialPrice[i].beginning_date <= dt_start_time && specialPrice[i].end_date <= dt_end_time)
                    {
                        temp = specialPrice[i].end_date.Subtract(dt_start_time);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                    else
                    {
                        temp = dt_end_time.Subtract(dt_start_time);
                        timespace = temp.Days;
                        p = p + specialPrice[i].price * timespace;
                        dayDiff = dayDiff - timespace;
                    }
                }
                p = p + dayDiff * Convert.ToDouble(Request.Form["price"]);
                double fee = p * 5 / 100;
                */
                var dataOrder = new HomestayOrder()
                {
                    status = -1,
                    customer_id = Session["user_id"].ToString(),
                    homestay_id = Convert.ToInt32(Request.Form["homestay_id"]),
                    create_time = dt_create,
                    start_time = dt_start_time,
                    end_time = dt_end_time,
                    total_price = Convert.ToDouble(Request.Form["price"]),
                    platform_fee = Convert.ToDouble(Request.Form["fee"]),
                    payment_method = Request.Form["payment"]
                };
                if (db.Insertable(dataOrder).ExecuteCommand() == 1)
                {
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "创建订单失败，请重试！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败！");
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }
        
    }
}