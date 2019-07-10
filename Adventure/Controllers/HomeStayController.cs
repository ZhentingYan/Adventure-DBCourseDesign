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
    public class HomeStayController : Controller
    {
        // GET: HomeStay
        public ActionResult Index()
        {
            return View();
        }


        // GET: Single Homestay

        public ActionResult StayInfo(int productID = -1)
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
                var getHomestaySpecific = db.Queryable<Homestay>().InSingle(productID);
                ViewBag.HomestayDetail = getHomestaySpecific;
                ViewBag.Landlord = db.Queryable<User>().InSingle(getHomestaySpecific.user_id);
            }
            catch (Exception ex)
            {
                Session["message"] = "无此房源！";
                return Redirect("~/Home");
            }
            finally { }
            List<int> list = null;
            if (Session["track_1"] == null)
            {
                list = new List<int>();
            }
            else
            {
                list = (List<int>)Session["track_1"];
            }

            if (list.Contains(productID))
                list.Remove(productID);    // 如果这次浏览的商品在浏览记录中,删除后重新添加进去,保持浏览顺序
            if (list.Count == 10)
                list.RemoveAt(0);    // 浏览记录保存10个,到了10个删除最老的一个记录.
            list.Add(productID);           // 浏览顺序为list[9] list[8].....list[1] list[0]
            Session["track_1"] = list;    // 把list更新到Session 
            return View();
        }
        // GET: Publish Homestay
        public ActionResult Release()
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
                    var myHomestayList = db.Queryable<Homestay>().Where(it => it.user_id == Session["user_id"]).ToArray();
                    ViewBag.homestayList = myHomestayList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                ViewBag.Message = "Publish your own homestay page.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult ReleaseAccess()
        {
            var address = Request.Form["form-address"];
            DateTime dt = DateTime.Now;
            string folder = Server.MapPath("~/images/Homestay");
            var filePath = folder + "\\" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);
            var fileSave = Request.Files["myfile"].FileName;
            if (Request.Files["myfile"].FileName != "")
            {
                fileSave = "../images/Homestay/" + dt.ToFileTime().ToString() + Request.Files["myfile"].FileName.Substring(Request.Files["myfile"].FileName.LastIndexOf("\\") + 1);

            }
            else
            {
                fileSave = "../iamges/Homestay/default.jpg";
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

            
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            DateTime dt_last = Convert.ToDateTime(Request.Form["form-latest_schedulable_date"], dtFormat);
            

            try
            {
                var data = new Homestay()
                {
                    user_id = (string)Session["user_id"],
                    homestay_name = Request.Form["form-homestay_name"],
                    introduction = Request.Form["form-introduction"],
                    num_of_bedrooms = Convert.ToInt16(Request.Form["form-num_of_bedrooms"]),
                    num_of_beds = Convert.ToInt16(Request.Form["form-num_of_beds"]),
                    num_of_bathrooms = Convert.ToInt16(Request.Form["form-num_of_bathrooms"]),
                    max_member_limit = Convert.ToInt16(Request.Form["form-max_member_limit"]),
                    default_price = Convert.ToInt16(Request.Form["form-default_price"]),
                    address = Request.Form["form-address"],
                    homestay_pictures = fileSave,
                    house_regulations = Request.Form["form-house_regulations"],
                    cancellation_policy = Request.Form["form-cancellation_policy"],
                    latest_schedulable_date = dt_last,
                    check_in_method = Request.Form["form-check_in_method"],
                    convenience_facilities = Request.Form["form-convenience_facilities"],
                    homestay_type = Request.Form["form-homestay_type"],

                };
                if (db.Insertable(data).ExecuteCommand() >= 1)
                {
                    int productID = db.Queryable<Homestay>().Max(it => it.homestay_id);
                    Session["message"] = "房源发布成功！房源ID:" + productID;
                    return Redirect("~/HomeStay/StayInfo?productID=" + productID);
                }
                else
                {
                    Session["message"] = "房源发布失败";
                    return Redirect("~/HomeStay/Release");
                }
            }
            catch (Exception ex)
            {
                Session["message"] = "房源发布失败";
                return Redirect("~/Homestay/Release");

            }
            finally { }
        }

        public ActionResult Special()
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
        public ActionResult AddFavourites(int productID = -1)
        {
            JObject isSuccess = new JObject();

            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                });
            if (Session["user_id"] == null || productID == -1)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "添加失败！你还没有登陆！");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
            }
            else
            {
                DateTime dt_create = DateTime.Now;
                var queryResult = db.Queryable<HomestayFavorite>().Where(it => it.user_id == (string)Session["user_id"] && it.homestay_id == productID).ToArray();
                if (queryResult.Length == 0)
                {
                    try
                    {
                        var data = new HomestayFavorite()
                        {
                            user_id = (string)Session["user_id"],
                            homestay_id = productID,
                            times = dt_create

                        };
                        if (db.Insertable(data).ExecuteCommand() >= 1)
                        {

                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "添加成功！");
                            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                        }
                        else
                        {

                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "添加失败！这个活动已经在你的心愿单了!");
                            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "添加失败！!");
                        return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                    }
                    finally { }
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "添加失败！这个活动已经在你的心愿单了!");
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                }
            }
        }
        [HttpPost]
        public ActionResult CheckAvailable()
        {
            JObject isSuccess = new JObject();

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

                DateTime dt_start_time, dt_end_time, dt_latest_time;
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy/MM/dd";
                dt_start_time = Convert.ToDateTime(Request.Form["start_date"], dtFormat);
                dt_end_time = Convert.ToDateTime(Request.Form["end_date"], dtFormat);
                dt_latest_time = Convert.ToDateTime(Request.Form["latest_time"], dtFormat);
                //int numNeed = Convert.ToInt32(Request.Form["form-member-num"]);


                var available = db.Queryable<HomestayOrder>().Where(it => it.homestay_id == Convert.ToInt32(Request.Form["productID"]) && (DateTime.Compare(dt_start_time, it.end_time) < 0 && DateTime.Compare(dt_end_time, it.start_time) > 0) && it.status <= 0).ToArray();
                if (available.Length == 0 && DateTime.Compare(dt_end_time, dt_latest_time) <= 0)
                {
                    isSuccess.Add("isSuccess", true);
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "您选择的日期不可预订！");
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "您选择的日期不可预订！");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

            }
        }

    }
}