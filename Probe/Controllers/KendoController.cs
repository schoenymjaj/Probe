using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;

namespace Probe.Controllers
{
    public class KendoController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: Kendo
        public ActionResult Index()
        {
            var aList = db.Game.Select(g => g.Name).ToList();

            return View(aList);
        }
    }
}