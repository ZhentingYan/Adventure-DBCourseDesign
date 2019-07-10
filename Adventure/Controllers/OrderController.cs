using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adventure.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order

        [HttpGet]
        public ActionResult Index(int productID= -1,int instanceID = -1)
        {
            if (Session["user_id"] == null)

            {
                Session["message"] = "你还没有登陆哦！";
                return Redirect("~/Login/index");
            }
            else
            {
                if(productID==-1 || instanceID==-1)
                    return Redirect("~/Home");
                //先这样，做不下去了，乱七八糟
                ViewBag.productID = productID;
                ViewBag.instanceID = instanceID;
                return View();
            }

        }
    }
}