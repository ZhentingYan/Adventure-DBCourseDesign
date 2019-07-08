using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adventure.Controllers
{
    public class ActivityController : Controller
    {
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        // GET: Single Activity
        public ActionResult SingleActivity()
        {
            ViewBag.Message = "The activity for more information page.";

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
                ViewBag.Message = "Publish your own activity page.";
                return View();
            }
        }
    }
}