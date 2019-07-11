using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using Adventure.Models;


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
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });
            String connection = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"];
            decimal customerCount, homestayCount, activityCount, reviewCount;
            using (OracleConnection conn = new OracleConnection(connection))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("select count(*) from users", conn);//执行一条SQL语句
                customerCount = (decimal)cmd.ExecuteScalar();
                cmd = new OracleCommand("select count(*) from Homestay", conn);//执行一条SQL语句
                homestayCount = (decimal)cmd.ExecuteScalar();
                cmd = new OracleCommand("select count(*) from Activity", conn);//执行一条SQL语句
                activityCount = (decimal)cmd.ExecuteScalar();
                cmd = new OracleCommand("select count(*) from activity_comment", conn);//执行一条SQL语句
                reviewCount = (decimal)cmd.ExecuteScalar();
                cmd = new OracleCommand("select count(*) from homestay_comment", conn);//执行一条SQL语句
                reviewCount += (decimal)cmd.ExecuteScalar();
                conn.Close();
            }
            JsonMessage m = new JsonMessage();
            var jsonData = "{ \"customerCount\":\"" + customerCount.ToString() + "\", \"homestayCount\":\"" + homestayCount.ToString() + "\", \"activityCount\":\"" + activityCount.ToString() + "\", \"reviewCount\":\"" + reviewCount.ToString() + "\"}";
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
            SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                DbType = DbType.Oracle,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
            String connection = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"];
            decimal customerCount, homestayCount, activityCount, reviewCount;
            using (OracleConnection conn = new OracleConnection(connection))
            {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand("select count(*) from users", conn);//执行一条SQL语句
                    customerCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from Homestay", conn);//执行一条SQL语句
                    homestayCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from Activity", conn);//执行一条SQL语句
                    activityCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from activity_comment", conn);//执行一条SQL语句
                    reviewCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from homestay_comment", conn);//执行一条SQL语句
                    reviewCount += (decimal)cmd.ExecuteScalar();
                    conn.Close();
                    ViewBag.customerCount = customerCount;
                    ViewBag.homestayCount = homestayCount;
                    ViewBag.activityCount = activityCount;
                    ViewBag.reviewCount = reviewCount;

            }
            try
            {
                /*
                List<user_comment> review = db.Queryable<User, ActivityComment>((st, sc) => new object[] {
                JoinType.Inner,st.user_id==sc.user_id})
                .Select((st, sc) => new user_comment { user_id = st.user_id, comment_text = sc.comment_text,grade=sc.grade,head_icon = st.head_icon }).ToList();
                */
                 var recommend_house = db.Queryable<Homestay>().Where(it => it.house_grade >= 0.0).ToArray();
                var recommend_story = db.Queryable<Blog>().OrderBy(it => it.story_id, OrderByType.Desc).ToArray();
                var top_experience = db.Queryable<Activity>().OrderBy(it => it.activity_id,OrderByType.Desc).ToArray(); 
                ViewBag.top_experience = top_experience;
                ViewBag.recommend_house = recommend_house;
                ViewBag.recommend_story = recommend_story;
                //ViewBag.review = review.ToArray();
                List<RAreview> recommendA_review = db.Queryable<ActivityComment, User>((st, sc) => new object[] {
                JoinType.Inner,st.user_id==sc.user_id})
                .OrderBy(st => st.grade, OrderByType.Desc)
                .Select((st, sc) => new RAreview
                {
                    activity_order_id = st.activity_order_id,
                    grade = st.grade,
                    user_id = st.user_id,
                    comment_text = st.comment_text,
                    times = st.times,
                    head_icon = sc.head_icon,
                    first_name = sc.first_name,
                    last_name = sc.last_name,
                }).ToList();

                List<RHreview> recommendH_review = db.Queryable<HomestayComment, User>((st, sc) => new object[] {
                JoinType.Inner,st.user_id==sc.user_id})
                .OrderBy(st => st.grade, OrderByType.Desc)
                .Select((st, sc) => new RHreview
                {
                    homestay_order_id = st.homestay_order_id,
                    grade = st.grade,
                    user_id = st.user_id,
                    comment_text = st.comment_text,
                    times = st.times,
                    head_icon = sc.head_icon,
                    first_name = sc.first_name,
                    last_name = sc.last_name,
                }).ToList();
                ViewBag.reviewA = recommendA_review.ToArray();
                ViewBag.reviewH = recommendH_review.ToArray();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            ViewBag.title = "度假屋、民宿、体验与旅行故事";
            return View();
        }

        public class user_comment
        {
            public string user_id { get; set; }
            public string comment_text { get; set; }
            public double grade { get; set; }
            public string head_icon { get; set; }
        }
        public class HomeInfo
        {
            public List<user_comment> r { get; set; }
            public List<Homestay> h { get; set; }
            public List<Blog> s { get; set; }
        }
        public ActionResult About()
        {
            SqlSugarClient db = new SqlSugarClient(
    new ConnectionConfig()
    {
        ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
        DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });
                String connection = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"];
                decimal customerCount, homestayCount, activityCount, reviewCount;
                using (OracleConnection conn = new OracleConnection(connection))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand("select count(*) from users", conn);//执行一条SQL语句
                    customerCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from Homestay", conn);//执行一条SQL语句
                    homestayCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from Activity", conn);//执行一条SQL语句
                    activityCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from activity_comment", conn);//执行一条SQL语句
                    reviewCount = (decimal)cmd.ExecuteScalar();
                    cmd = new OracleCommand("select count(*) from homestay_comment", conn);//执行一条SQL语句
                    reviewCount += (decimal)cmd.ExecuteScalar();
                    conn.Close();
                    ViewBag.customerCount = customerCount;
                    ViewBag.homestayCount = homestayCount;
                    ViewBag.activityCount = activityCount;
                    ViewBag.reviewCount = reviewCount;

                }
                try
                {

                    List<RAreview> recommendA_review = db.Queryable<ActivityComment, User>((st, sc) => new object[] {
                    JoinType.Inner,st.user_id==sc.user_id})
                    .OrderBy(st => st.grade, OrderByType.Desc)
                    .Select((st, sc) => new RAreview
                    {
                        activity_order_id = st.activity_order_id,
                        grade = st.grade,
                        user_id = st.user_id,
                        comment_text = st.comment_text,
                        times = st.times,
                        head_icon = sc.head_icon,
                        first_name = sc.first_name,
                        last_name = sc.last_name,
                    }).ToList();

                    List<RHreview> recommendH_review = db.Queryable<HomestayComment, User>((st, sc) => new object[] {
                    JoinType.Inner,st.user_id==sc.user_id})
                    .OrderBy(st => st.grade, OrderByType.Desc)
                    .Select((st, sc) => new RHreview
                    {
                        homestay_order_id = st.homestay_order_id,
                        grade = st.grade,
                        user_id = st.user_id,
                        comment_text = st.comment_text,
                        times = st.times,
                        head_icon = sc.head_icon,
                        first_name = sc.first_name,
                        last_name = sc.last_name,
                    }).ToList();
                    ViewBag.reviewA = recommendA_review.ToArray();
                    ViewBag.reviewH = recommendH_review.ToArray();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                ViewBag.title = "关于我们";
                return View();
        }
        [HttpGet]
        public ActionResult Contact()
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
        public ActionResult Contact(FormCollection form)
        {

            return View();
        }
        public ActionResult Favourites()
        {
            if (Session["user_id"] == null)
                return Redirect("~/Home");
            SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                DbType = DbType.Oracle,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
            List<int> activityList = (List<int>)Session["track"];
            List<int> homestayList = (List<int>)Session["track_1"];
            List<Activity> list1 = new List<Activity>();
            List<Homestay> list2 = new List<Homestay>();
            try
            {
                if (activityList != null)
                {
                    for (int i = 0; i < activityList.ToArray().Length; i++)
                    {
                        var getActivityByID = db.Queryable<Activity>().InSingle(activityList[i]);
                        list1.Add(getActivityByID);
                    }
                }
                if (homestayList != null)
                {
                    for (int i = 0; i < homestayList.ToArray().Length; i++)
                    {
                        var getHomestayByID = db.Queryable<Homestay>().InSingle(homestayList[i]);
                        list2.Add(getHomestayByID);
                    }
                }
                ViewBag.activityList = list1.ToArray();
                ViewBag.homestayList = list2.ToArray();
                List<Activity> list3 = new List<Activity>();
                List<Homestay> list4 = new List<Homestay>();
                var getFavouriteHomestay = db.Queryable<HomestayFavorite>().Where(it => it.user_id == (string)Session["user_id"]).ToArray();
                var getFavouriteActivity = db.Queryable<ActivityFavorite>().Where(it => it.user_id == (string)Session["user_id"]).ToArray();

                if (getFavouriteActivity != null)
                {
                    for (int i = 0; i < getFavouriteActivity.Length; i++)
                    {
                        var getActivity = db.Queryable<Activity>().InSingle(getFavouriteActivity[i].activity_id);
                        list3.Add(getActivity);
                    }
                }
                if (getFavouriteHomestay != null)
                {
                    for (int i = 0; i < getFavouriteHomestay.Length; i++)
                    {
                        var getHomestay = db.Queryable<Homestay>().InSingle(getFavouriteHomestay[i].homestay_id);
                        list4.Add(getHomestay);
                    }
                }
                ViewBag.favouriteActivity = list3.ToArray();
                ViewBag.favouriteHomestay= list4.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            
            return View();
        }
        public class RAreview
        {
            public int activity_order_id { get; set; }
            public double grade { get; set; }
            public string user_id { get; set; }
            public string comment_text { get; set; }
            public DateTime times { get; set; }
            public string head_icon { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
        }
        public class RHreview
        {
            public int homestay_order_id { get; set; }
            public double grade { get; set; }
            public string user_id { get; set; }
            public string comment_text { get; set; }
            public DateTime times { get; set; }
            public string head_icon { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
        }
        [HttpPost]
        public ActionResult SubmitReport(FormCollection form)
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
            DateTime dt = DateTime.Now;
            try
            {
                int t = Convert.ToInt16(form["get_type"]);
                string tyPE;
                if (t == 1)
                {
                    tyPE = "体验活动订单";
                }
                else if (t == 2)
                {
                    tyPE = "民宿订单";
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "参数错误!");
                    return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
                }
                var ReportData = new Report()
                {
                    user_id = Session["user_id"].ObjToString(),
                    email = Session["email_address"].ToString(),
                    name = form["name"],
                    type = tyPE,
                    order_id = Convert.ToInt32(form["get_order_id"]),
                    reason = form["get_reason"],
                    times = dt
                };
                //先找homestay_order
                if (t == 2)//type与homestay匹配
                {
                    //bool db.Queryable<Homestay_Order>().Where(it => it.homestay_order_id == ReportData.order_id).Any()；
                    if (db.Queryable<HomestayOrder>().Any(it => it.homestay_order_id == ReportData.order_id && it.customer_id == ReportData.user_id)) //房屋订单有否                                                                                                                //此查找语句未经测试
                    {
                        if (db.Insertable(ReportData).ExecuteCommand() == 1)//插入成否
                        {
                            isSuccess.Add("isSuccess", true);
                        }
                        else
                        {
                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "反馈与意见上传失败!");
                        }
                    }
                    else
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "无法找到与您相关的订单!");
                    }
                }
                else if (t == 1)//type与activity匹配
                {
                    //bool db.Queryable<Homestay_Order>().Where(it => it.homestay_order_id == ReportData.order_id).Any()；
                    if (db.Queryable<ActivityOrder>().Any(it => it.activity_order_id == ReportData.order_id && it.customer_id == ReportData.user_id))//房屋订单有否
                    {
                        if (db.Insertable(ReportData).ExecuteCommand() == 1)//插入成否
                        {
                            isSuccess.Add("isSuccess", true);
                        }
                        else
                        {
                            isSuccess.Add("isSuccess", false);
                            isSuccess.Add("message", "反馈与意见上传失败!");
                        }
                    }
                    else
                    {
                        isSuccess.Add("isSuccess", false);
                        isSuccess.Add("message", "无法找到与您相关的订单!");
                    }
                }
                else//type没和任何值匹配上
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "无法匹配正确类型！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作错误！");
                return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
            }
            finally { };
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }


    }
}
