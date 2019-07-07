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
        public ActionResult Index(int productID= -1,int accessWay=-1)
        {
            if (Session["user_id"] == null)
            {
                return Redirect("~/Login/index");
            }
            else
            {
                ViewBag.productID = productID;
                ViewBag.accessWay = accessWay;
                return View();
            }

        }
    }
}