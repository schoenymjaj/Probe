using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public ActionResult About()
        {
            ViewBag.Message = "This is the Probe Internet Game Application. " +
            "An authorized user (registered and logged in) may configure and publish a game to be played on any device connected to the internet. " +
            "A game requires all players to answer a series of questions designed by the user and is contested by the specific rules of the game type.  " +
            "Current game types available are 'Match' and 'Test'. Devices supported are iPhone, Android, and Windows phone.";
            

            return View();
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