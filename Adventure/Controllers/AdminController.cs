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
        //我订别人的房源订单

        [HttpGet]
        public ActionResult checkMyHOrder()
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
                try
                {
                    //查询我预定活动结果
                    var myHomestayList = db.Queryable<HomestayOrder>().Where(it => it.customer_id == Session["user_id"].ToString()).ToArray();
                    ViewBag.returnlist = myHomestayList;
                    ViewBag.isSuccess = 1;
                }
                catch (Exception ex)
                {
                    ViewBag.returnlist = null;
                    return View();
                }
                return View();
            }
        }
        //别人订我的房源订单
        [HttpGet]
        public ActionResult checkMyHomestayOrder()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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
                try
                {
                    //查询我预定活动结果
                    var bookedList = db.Queryable<HomestayOrder, Homestay>((st, sc) => new object[] {
                JoinType.Inner,st.homestay_id==sc.homestay_id,
                })
                    .OrderBy((st) => st.create_time)
                    .Where((st, sc) => sc.user_id == Session["user_id"].ToString())
                    .Select((st, sc) => new
                    {
                        homestay_order_id = st.homestay_order_id,
                        customer_id = st.customer_id,
                        homestay_id = st.homestay_id,
                        status = st.status,
                        create_time = st.create_time,
                        start_time = st.start_time,
                        end_time = st.end_time,
                        total_price = st.total_price,
                        platform_fee = st.platform_fee,
                        homestay_name = sc.homestay_name
                    }).ToArray();
                    List<Ordered_Homestay_Return> returnList = new List<Ordered_Homestay_Return>();
                    for (int i = 0; i < bookedList.Length; i++)
                    {
                        Ordered_Homestay_Return temp = new Ordered_Homestay_Return
                        {
                            homestay_order_id = bookedList[i].homestay_order_id,
                            customer_id = bookedList[i].customer_id,
                            homestay_id = bookedList[i].homestay_id,
                            status = bookedList[i].status,
                            create_time = bookedList[i].create_time,
                            start_time = bookedList[i].start_time,
                            end_time = bookedList[i].end_time,
                            total_price = bookedList[i].total_price,
                            platform_fee = bookedList[i].platform_fee,
                            homestay_name = bookedList[i].homestay_name
                        };
                        returnList.Add(temp);
                    }
                    ViewBag.isSuccess = 1;
                    ViewBag.returnList = returnList.ToArray();
                }
                catch (Exception ex)
                {
                    ViewBag.isSuccess = 0;
                    throw ex;
                }
                return View();
            }
        }

        public class Ordered_Homestay_Return
        {
            public int homestay_order_id { get; set; }
            public string customer_id { get; set; }
            public int homestay_id { get; set; }
            public int status { get; set; }
            public DateTime create_time { get; set; }
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public double total_price { get; set; }
            public double platform_fee { get; set; }
            public string homestay_name { get; set; }



        }
      
        public ActionResult ViewActivityInstance()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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

                try
                {

                    var activityResult = db.Queryable<ActivityInstance, Activity>((st, sc) => new object[] {
                JoinType.Inner,st.activity_id==sc.activity_id})
                    .Where((st, sc) => sc.user_id == Session["user_id"].ToString())
                    .Select((st, sc) => new
                    {
                        activity_id = st.activity_id,
                        activity_name = sc.activity_name,
                        activity_instance_id = st.activity_instance_id,
                        start_time = st.start_time,
                        end_time = st.end_time,
                        is_booked = st.is_booked,
                        price = st.price
                    })
                    .ToArray();
                    List<ActivityInstanceReturn> returnList = new List<ActivityInstanceReturn>();
                    for (int i = 0; i < activityResult.Length; i++)
                    {
                        ActivityInstanceReturn temp = new ActivityInstanceReturn
                        {
                            activity_id = activityResult[i].activity_id,
                            activity_name = activityResult[i].activity_name,
                            activity_instance_id = activityResult[i].activity_instance_id,
                            start_time = activityResult[i].start_time,
                            end_time = activityResult[i].end_time,
                            is_booked = activityResult[i].is_booked,
                            price = activityResult[i].price
                        };
                        returnList.Add(temp);
                    }
                    ViewBag.isSuccess = 1;
                    ViewBag.returnList = returnList.ToArray();

                    //ViewBag.activityList = activityResult;
                }
                catch (Exception ex)
                {
                    ViewBag.returnList = null;
                    return View();
                }
                finally { }
                return View();
            }
        }
        public class ActivityInstanceReturn
        {
            public int activity_id { get; set; }
            public string activity_name { get; set; }
            public int activity_instance_id { get; set; }
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public int is_booked { get; set; }
            public double price { get; set; }

        }
        public ActionResult ViewHomestay()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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
                try
                {
                    var homestayList = db.Queryable<Homestay>().Where(it => it.user_id == (string)Session["user_id"]).ToArray();
                    ViewBag.homestayList = homestayList;
                }
                catch
                {
                    return View();
                }
                return View();
            }
        }
        public ActionResult ViewActivity()
        {
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
                var activityList = db.Queryable<Activity>().Where(it => it.user_id == (string)Session["user_id"]).ToArray();
                ViewBag.activityList = activityList;
            }
            catch
            {
                return View();
            }
            return View();
        }
        //删除
        [HttpPost]
        public ActionResult RemoveActivityInstance()
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
                if (db.Deleteable<ActivityInstance>().Where(it => it.is_booked == 0 && it.activity_instance_id == Convert.ToInt32(Request.Form["productID"])).ExecuteCommand() == 1)
                {
                    isSuccess.Add("isSuccess", true);
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "删除失败，请检查您的活动实例是否被预定。");
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "删除失败，请检查您的活动实例是否被预定。");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
            }
            finally { }
        }

        [HttpGet]
        public ActionResult checkMyActivityOrder()
        {
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
                //查询我预定活动结果
                var bookedList = db.Queryable<ActivityOrder, ActivityInstance, Activity>((st, sc, st2) => new object[] {
                JoinType.Inner,sc.activity_instance_id==st.activity_instance_id,
                JoinType.Inner,sc.activity_id==st2.activity_id
                })
                .Where((st, sc, st2) => st2.user_id == Session["user_id"].ToString())
                .OrderBy((st) => st.create_time)
                .Select((st, sc, st2) => new {
                    activity_id = st2.activity_id,
                    activity_name = st2.activity_name,
                    activity_order_id = st.activity_order_id,
                    customer_id = st.customer_id,
                    status = st.status,
                    create_time = st.create_time,
                    start_time = st.start_time,
                    end_time = st.end_time,
                    total_price = st.total_price,
                    platform_fee = st.platform_fee
                }).ToArray();
                List<Ordered_Activity_Return> returnList = new List<Ordered_Activity_Return>();
                for(int i = 0; i < bookedList.Length; i++)
                {
                    Ordered_Activity_Return temp = new Ordered_Activity_Return
                    {
                        activity_id = bookedList[i].activity_id,
                        activity_name = bookedList[i].activity_name,
                        activity_order_id = bookedList[i].activity_order_id,
                        customer_id = bookedList[i].customer_id,
                        status = bookedList[i].status,
                        create_time = bookedList[i].create_time,
                        start_time = bookedList[i].start_time,
                        end_time = bookedList[i].end_time,
                        total_price = bookedList[i].total_price,
                        platform_fee = bookedList[i].platform_fee
                    };
                    returnList.Add(temp);
                }
                ViewBag.isSuccess = 1;
                ViewBag.returnList = returnList.ToArray();
            }
            catch (Exception ex)
            {
                ViewBag.isSuccess = 0;
                return View();

            }
            return View();
        }
        T Cast<T>(object obj, T type)
        {
            return (T)obj;
        }
        //删除活动
        public class Ordered_Activity_Return {
            public int activity_id{get;set;}
            public string activity_name { get; set; }
            public int activity_order_id { get; set; }
            public string customer_id { get; set; }
            public int status { get; set; }
            public DateTime create_time { get; set; }
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public double total_price { get; set; }
            public double platform_fee { get; set; }

        }

        [HttpPost]
        public ActionResult RemoveActivity()
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
                //找到是否有未预定或者已预定且未完成的订单
                var checkif = db.Queryable<ActivityOrder, ActivityInstance>((st, sc) => new object[] {
                JoinType.Inner,st.activity_instance_id==sc.activity_instance_id})
                .Where((st, sc) => sc.activity_id == Convert.ToInt32(Request.Form["productID"]) && (sc.is_booked == 0 || (sc.is_booked == 1 && st.status <= 0)))
                .Select((st, sc) => new { Id = sc.activity_id }).ToArray();
                if (checkif.Length == 0)
                {
                    db.Deleteable<ActivityInstance>(it => it.activity_id == Convert.ToInt32(Request.Form["productID"])).ExecuteCommand();
                    db.Deleteable<Activity>().In(Convert.ToInt32(Request.Form["productID"])).ExecuteCommand();
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "不允许删除！你有未被预定的活动实例/你的实例正在处于活跃状态！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }

       
        [HttpPost]
        public ActionResult RemoveHomestay(FormCollection form)
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
                //检测房源是否有正处在活跃的订单
                var checkif = db.Queryable<HomestayOrder>().Where(it => it.homestay_id == Convert.ToInt32(Request.Form["productID"]) && it.status <= 0).ToArray();
                if (checkif.Length == 0)
                {
                    db.Deleteable<SpecialPrice>(it => it.homestay_id == Convert.ToInt32(Request.Form["productID"])).ExecuteCommand();
                    db.Deleteable<Homestay>().In(Convert.ToInt32(Request.Form["productID"])).ExecuteCommand();
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "不允许删除！你有房源订单正在处于活跃状态！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }
        
        public ActionResult Comment(int orderID=-1,int accessWay=-1,int productID=-1)
        {
            if (orderID == -1 || accessWay == -1 || productID == -1)
            {
                Session["message"] = "非法访问评论页面！";
                return Redirect("~/Home");
            }
            else if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
            }
            else
            {
                ViewBag.orderID = orderID;
                ViewBag.accessWay = accessWay;
                ViewBag.productID = productID;
                return View();
            }
        }

        public ActionResult CheckComment()
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
                if (Convert.ToInt16(Request.Form["accessWay"]) == 2)
                {
                    var checkStatus = db.Queryable<HomestayOrder>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                    if (checkStatus.status == 1)
                    {
                        var isComment = db.Queryable<HomestayOrder>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                        if (isComment == null)
                        {
                            isSuccess.Add("isSuccess", true);
                        }
                        else
                        {
                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "您已评价过此订单，谢谢！");
                        }
                    }
                    else
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "此订单未完成，无法添加评价！");
                    }
                }
                else if (Convert.ToInt16(Request.Form["accessWay"]) == 1)
                {
                    var checkStatus = db.Queryable<ActivityOrder>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                    if (checkStatus.status == 1)
                    {
                        var isComment = db.Queryable<ActivityComment>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                        if (isComment == null)
                        {
                            isSuccess.Add("isSuccess", true);
                        }
                        else
                        {
                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "您已评价过此订单，谢谢！");
                        }
                    }
                    else
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "此订单未完成，无法添加评价！");
                    }
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "参数出错！");
                }
            }

            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败！");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));

            }
            finally
            {
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }

        [HttpPost]
        public ActionResult submitComment(FormCollection form)
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
            }
            else
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
                    DateTime dt = DateTime.Now;
                    var ActComData = new ActivityComment()
                    {
                        activity_order_id = Convert.ToInt32(Request.Form["orderID"]),
                        grade = Convert.ToDouble(Request.Form["rate"]),
                        user_id = Session["user_id"].ToString(),
                        comment_text = Request.Form["comment_content"],
                        times = dt
                    };
                    if (Convert.ToInt16(Request.Form["accessWay"]) == 2)
                    {
                        if (db.Insertable<HomestayComment>(ActComData).ExecuteCommand() == 1)
                        {
                            Session["message"] = "订单评价成功!";
                            return Redirect("~/Admin");
                        }
                        else
                        {
                            Session["message"] = "添加评论失败!";
                            return Redirect("~/Admin");

                        }
                    }
                    else if (Convert.ToInt16(Request.Form["accessWay"]) == 1)
                    {
                        if (db.Insertable<ActivityComment>(ActComData).ExecuteCommand() == 1)
                        {
                            Session["message"] = "订单评价成功!";
                            return Redirect("~/Admin");
                        }
                        else
                        {
                            Session["message"] = "添加评论失败!";
                            return Redirect("~/Admin");
                        }
                    }
                    else
                    {
                        Session["message"] = "参数错误!";
                        return Redirect("~/Admin");
                    }
                }
                catch (Exception ex)
                {
                    Session["message"] = "操作失败!";
                    return Redirect("~/Admin"); 

                }
                finally {
                }
            }
        }

        /*
        [HttpPost]
        public ActionResult submitComment()
        {

        }
        */


        [HttpPost]
        public ActionResult CancelActivityOrder(FormCollection form)
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
                var id = Convert.ToInt32(Request.Form["orderID"]);
                var target = db.Queryable<ActivityOrder>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                if (target.status == -1)
                {
                    target.status = -2;
                    db.Updateable(target).UpdateColumns(it => new { it.status }).ExecuteCommand();
                    var relatedActivityIns = db.Queryable<ActivityInstance>().InSingle(target.activity_instance_id);
                    relatedActivityIns.is_booked = 0;
                    db.Updateable(relatedActivityIns).UpdateColumns(it => new { it.is_booked }).ExecuteCommand();
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "你的订单不处在待出行状态，无法取消");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败");
                throw ex;
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }

        //取消房源订单


        [HttpPost]
        public ActionResult CancelhomestayOrder()
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
                //查询房源订单结果
                var target = db.Queryable<HomestayOrder>().InSingle(Convert.ToInt32(Request.Form["orderID"]));
                if (target.status == -1)
                {
                    target.status = -2;
                    db.Updateable(target).UpdateColumns(it => new { it.status }).ExecuteCommand();
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "你的房源订单不处在待出行状态，无法取消");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }
        [HttpGet]
        public ActionResult checkMyAOrder()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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
                try
                {
                    //查询我预定活动结果
                    var myActivityList = db.Queryable<ActivityOrder>().Where(it => it.customer_id == Session["user_id"].ToString()).ToArray();
                    ViewBag.isSuccess = 1;
                    ViewBag.returnList = myActivityList;
                }
                catch (Exception ex)
                {
                    ViewBag.isSuccess = 0;
                    ViewBag.returnList = null;
                    return View();

                }
                return View();
            }
        }

        public ActionResult ViewBlog()//后台浏览旅行故事
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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
                try
                {
                    var myBlogList = db.Queryable<Blog>().Where(it => it.user_id == Session["user_id"]).ToArray();
                    ViewBag.returnList = myBlogList;
                }
                catch (Exception ex)
                {
                    ViewBag.returnList = null;
                    return View();
                }
                finally { }
                return View();
            }
        }

        //删除特殊价格
        [HttpPost]
        public ActionResult RemoveSpecialPrice()
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
                if (db.Deleteable<SpecialPrice>().In(Convert.ToInt32(Request.Form["instanceID"])).ExecuteCommand() == 1)
                {
                    isSuccess.Add("isDelete", true);
                }
                else
                {
                    isSuccess.Add("isDelete", false);
                    isSuccess.Add("message", "特殊价格删除失败！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isDelete", false);
                isSuccess.Add("message", "特殊价格删除失败！");
                throw ex;
            }
            finally { }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }

        //浏览特殊价格
        public ActionResult ViewHomestaySpecial()
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/login/index");
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
                try
                {
                    List<HomestaySP> list = db.Queryable<SpecialPrice, Homestay>((st, sc) => new object[] {
                JoinType.Inner,st.homestay_id==sc.homestay_id})
                    .Select((st, sc) => new HomestaySP
                    {
                        special_price_id = st.special_price_id,
                        homestay_id = st.homestay_id,
                        beginning_date = st.beginning_date,
                        end_date = st.end_date,
                        price = st.price,
                        homestay_name = sc.homestay_name
                    }).ToList();
                    ViewBag.returnList = list.ToArray();
                }
                catch (Exception ex)
                {
                    ViewBag.returnList = null;
                    return View();
                }
                finally { }
                return View();
            }
        }
        public class HomestaySP
        {
            public int special_price_id { get; set; }
            public int homestay_id { get; set; }
            public DateTime beginning_date { get; set; }
            public DateTime end_date { get; set; }
            public double price { get; set; }
            public string homestay_name { get; set; }
        }
    }

}