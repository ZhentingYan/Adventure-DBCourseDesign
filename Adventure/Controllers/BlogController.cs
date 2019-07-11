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
    public class BlogController : Controller
    {
        // GET: Blog

        public ActionResult Index()
        {
           
                ViewBag.title = "旅行故事";
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
                    var getAllStory = db.Queryable<Blog>().OrderBy(it => it.story_id, OrderByType.Desc).ToArray();
                    ViewBag.AllBlogs = getAllStory;//发送
                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "加载失败!";
                    throw ex;
                }
                finally { }
                return View();
            
        }


        public ActionResult NewBlog()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ChangeBlog(FormCollection form)
        {
            DateTime nowtime = DateTime.Now;
            string folder = Server.MapPath("~/images/Blog");
            var filePath = folder + "\\" + nowtime.ToFileTime().ToString() + Request.Files["changeBlogPicture"].FileName.Substring(Request.Files["changeBlogPicture"].FileName.LastIndexOf("\\") + 1);
            var fileSave = Request.Files["changeBlogPicture"].FileName;
            if (Request.Files["changeBlogPicture"].FileName != "")
            {
                fileSave = "../images/Blog/" + nowtime.ToFileTime().ToString() + Request.Files["changeBlogPicture"].FileName.Substring(Request.Files["changeBlogPicture"].FileName.LastIndexOf("\\") + 1);
            }
            else
            {
                fileSave = (string)Session["blog_pic"];
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Request.Files["changeBlogPicture"].SaveAs(filePath);
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
                var updata = new Blog
                {
                    story_id = Convert.ToInt16(Request.Form["blog_ID"]),
                    user_id = (string)Session["user_id"],
                    title = Request.Form["changeBlogTitle"],
                    content_s = Request.Form["changeBlogContent"],
                    pictures = fileSave,
                    times = nowtime
                };
                if (db.Updateable(updata).ExecuteCommand() == 1)
                {
                    ViewBag.flag = 1;
                }
                else
                {
                    ViewBag.errorMessage = "旅行故事修改失败!";
                    ViewBag.flag = 0;
                }
                return Redirect("~/Blog/BlogDetail?BlogId=" + Request.Form["blog_ID"]);
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "旅行故事修改失败!";
                ViewBag.flag = 0;
                return Redirect("~/Blog/BlogDetail?BlogId=" + Request.Form["blog_ID"]);
            }
        }


        public ActionResult BlogDetail(int BlogId = -1)//加载详细story
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
                    var theDetail = db.Queryable<Blog>().InSingle(BlogId);
                    var theAuthor = db.Queryable<User>().InSingle(theDetail.user_id);
                    ViewBag.CurrentBlog = theDetail;
                    ViewBag.Author = theAuthor;
                    Session["blog_pic"] = theDetail.pictures;
                    if (theAuthor.user_id != (string)Session["user_id"])
                        ViewBag.flag = 0;
                    else
                        ViewBag.flag = 1;
                }
                catch (Exception ex)
                {
                    Session["message"] = "无此旅行故事!";
                    //ViewBag.flag = 0;
                    return Redirect("~/Home");
                }
                finally { }
                return View();
            }
        }

        public ActionResult deleteBlog()
        {
            //删除旅行故事
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });
            JObject isSuccess = new JObject();
            int sID = Request.Form["story_id"].ObjToInt();
            try
            {
                if (db.Deleteable<Blog>().In(sID).ExecuteCommand() == 1)
                {
                    isSuccess.Add("isSuccess", true);
                }
                else
                {
                    isSuccess.Add("isSuccess", false);
                    isSuccess.Add("message", "删除旅行故事失败！");
                }
            }
            catch (Exception ex)
            {
                isSuccess.Add("isSuccess", false);
                isSuccess.Add("message", "操作失败！");
                throw ex;

            }
            return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }


        public ActionResult SubmitBlog(FormCollection form)         //发布故事
        {
            DateTime nowtime = DateTime.Now;
            string folder = Server.MapPath("~/images/Blog");
            var filePath = folder + "\\" + nowtime.ToFileTime().ToString() + Request.Files["blogPicture"].FileName.Substring(Request.Files["blogPicture"].FileName.LastIndexOf("\\") + 1);
            var fileSave = Request.Files["blogPicture"].FileName;
            if (Request.Files["blogPicture"].FileName != "")
            {
                fileSave = "../images/Blog/" + nowtime.ToFileTime().ToString() + Request.Files["blogPicture"].FileName.Substring(Request.Files["blogPicture"].FileName.LastIndexOf("\\") + 1);
            }
            else
            {
                fileSave = "../iamges/Blog/default.jpg";
            }
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Request.Files["blogPicture"].SaveAs(filePath);
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
                var storyData = new Blog()
                {
                    user_id = (string)Session["user_id"],
                    title = form["blogTitle"],
                    content_s = form["blogContent"],
                    pictures = fileSave,
                    times = nowtime
                };

                if (db.Insertable(storyData).ExecuteCommand() == 1)
                {
                    ViewBag.flag = 1;
                    int storyId = db.Queryable<Blog>().Max(it => it.story_id);//最大值
                    return Redirect("../Blog/BlogDetail?BlogId=" + storyId);
                }
                else
                {
                    ViewBag.errorMessage = "旅行故事发布失败!";
                    ViewBag.flag = 0;
                }
            }

            catch (Exception ex)
            {
                ViewBag.errorMessage = "操作失败!";
                ViewBag.flag = 0;
                throw ex;

            }
            finally { }
            return View();
        }
    }
}

