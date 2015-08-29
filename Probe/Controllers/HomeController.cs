using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProbeDAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Probe.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About(int? mobileind)
        {

            if (mobileind == null || mobileind == 0)
            {
                ViewBag.MobileInd = false;
                return View("About");
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("About", "_MobileLayout");
            }

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Info()
        {
            ViewBag.Message = "Your Probe Info page.";

            return View();
        }
    }
}