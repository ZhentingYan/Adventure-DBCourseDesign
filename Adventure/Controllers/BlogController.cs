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
            if (Session["user_id"] == null)
            {

                return Redirect("~/login/index");
            }
            else
            {
                ViewBag.title = "旅行故事";
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
                    //int max = db.Queryable<Blog>().Max(it => it.story_id);//最大值
                    var getAllOrder = db.Queryable<Blog>().OrderBy(it => it.story_id, OrderByType.Desc).ToArray();
                    //发送全部的倒序
                    // var top5 = db.Queryable<Blog>().Take(5).ToList();    //前五条    
                    ViewBag.AllBlogs = getAllOrder;//发送
                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "加载失败!";
                    return View();
                }
                finally { }
                return View();
            }
        }


        public ActionResult NewBlog()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ChangeBlog(FormCollection form)
        {
            int changestoryid = 0;
            string flag = form["changeBlogSubmit"];
            if (flag == null)
            {
                changestoryid = Request.Form["changeblogiiD"].ObjToInt();
            }
            if (flag != null)
            {
                SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"],
                    DbType = DbType.Oracle,//设置数据库类型
                         IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                         InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                     });


                // JObject isSuccess = new JObject();


                var ChangeDetail = db.Queryable<Blog>().InSingle(changestoryid);
                string changeUserId = ChangeDetail.user_id;
                DateTime nowtime = DateTime.Now;
                var updata = new Blog
                {
                    story_id = changestoryid,
                    user_id = changeUserId,
                    title = form["changeBlogTitle"],
                    content_s = form["changeBlogPosition"],
                    pictures = "image/*",
                    times = nowtime
                };
                if (db.Insertable(updata).ExecuteCommand() == 1)
                {
                    ViewBag.errorMessage = "旅行故事修改成功!";
                    //  ViewBag.flag = 1;
                    return Content(ViewBag.errorMessage);
                }
                else
                {
                    ViewBag.errorMessage = "旅行故事修改失败!";
                    // ViewBag.flag = 0;
                    return Content(ViewBag.errorMessage);
                }

            }
            else
            {
                ViewBag.errorMessage = "旅行故事修改失败!";
                // ViewBag.flag = 0;
                return Content(ViewBag.errorMessage);
            }


        }


        public ActionResult BlogDetail(int BlogId = -1)//加载详细story
        {

            //  ViewBag.title = "旅行故事";
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
                //int max = db.Queryable<Blog>().Max(it => it.story_id);
                // var getStoryId = db.Queryable<Blog>().Where(it => it.story_id == BlogId).ToList();

                var theDetail = db.Queryable<Blog>().InSingle(BlogId);
                var theAuthor = db.Queryable<User>().InSingle(theDetail.user_id);

                ViewBag.CurrentBlog = theDetail;
                ViewBag.Author = theAuthor;
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "加载失败!";
                //ViewBag.flag = 0;
                return View();
            }
            finally { }
            return View();
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
                        InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                    });
            //int max = db.Queryable<Blog>().Max(it => it.story_id);
            // var getStoryId = db.Queryable<Blog>().Where(it => it.story_id == BlogId).ToList();

            int a = Request.Form["story_id"].ObjToInt();
            //var t3 = db.Deleteable<Blog>().In(a).ExecuteCommand();
            var t3 = db.Deleteable<Blog>().Where(new Blog() { story_id = a }).ExecuteCommand();
            //int asd = 0;

            if (t3 == 1)
            {
                ViewBag.errorMessage = "删除成功!";

                return Redirect("~/Blog/Index");
            }
            //  ViewBag.flag = 0;

            else
            {
                ViewBag.errorMessage = "删除失败!";
                return Redirect("~/Blog/Index");
            }


            //return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented))

        }


        public ActionResult SubmitBlog(FormCollection form)         //发布故事
        {
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
                int max = db.Queryable<Blog>().Max(it => it.story_id);//最大值
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
                var storyData = new Blog()
                {
                    user_id = Session["user_id"].ObjToString(),
                    title = form["blogTitle"],
                    content_s = form["blogContent"],
                    pictures = fileSave,
                    times = nowtime
                };

                if (db.Insertable(storyData).ExecuteCommand() == 1)
                {
                    ViewBag.errorMessage = "旅行故事发布成功!";
                    ViewBag.flag = 1;
                    var storyId = max + 1;
                    return Redirect("../Blog/BlogDetail?BlogId=" + storyId);

                }
                else
                {
                    ViewBag.errorMessage = "旅行故事发布失败!";
                    ViewBag.flag = 0;
                    return Content(ViewBag.errorMessage);
                }
            }

            catch (Exception ex)
            {
                ViewBag.errorMessage = "注册失败!";
                ViewBag.flag = 0;
                return Content(ViewBag.errorMessage);
            }
            finally { }
            //return Content(JsonConvert.SerializeObject(isSuccess, Formatting.Indented));
        }
    }
}

