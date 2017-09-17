using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProbeDAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Mics;
using Probe.DAL;

namespace Probe.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ProbeDataContext db = new ProbeDataContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About(int? id)
        {
            //no id passed means the about section offset to be expanded will be zero
            if (id == null)
            {
                id = 0;
            }
            else if (id > ProbeConstants.MaxAboutSections - 1)
            {
                id = 0;
            }

            ViewBag.InCommonVersion = db.ConfigurationG.Where(c => c.Name == "InCommon-Version").FirstOrDefault().Value;
            ViewBag.AboutSectionOffset = id;
            return View("About");
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