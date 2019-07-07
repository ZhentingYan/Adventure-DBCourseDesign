using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adventure.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                Session["user"] = 123456;
                return Redirect("~/Login/index");
            }
            else
            {
                ViewBag.title = "旅行故事";
                return View();
            }
        }
    }
}