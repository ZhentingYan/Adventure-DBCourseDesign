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
        public ActionResult Index(int homeStayID=-1)
        {
            ViewBag.homestayID = homeStayID;
            return View();
        }
    }
}