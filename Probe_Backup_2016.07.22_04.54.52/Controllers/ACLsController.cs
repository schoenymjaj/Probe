using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Helpers.Mics;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Probe.Controllers
{
    public class ACLsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        /*
         * Get all Game Types
         */
        public JsonResult Get(int probeacltype)
        {
            try
            {

                /*
                 * This get all ACLs API is very question specific 
                 */

                List<ACL> aclList = db.ACL.ToList();

                switch ((ProbeACLType)probeacltype)
                {
                    case ProbeACLType.QUESTION:
                        aclList.Where(a => a.Name == "PRIVATE").FirstOrDefault().Description = "My Questions";
                        aclList.Where(a => a.Name == "GLOBAL").FirstOrDefault().Description = "Shared Questions";
                        aclList.Add(new ACL { Id = 0, Description = "All Questions" });
                        aclList.Sort((a1, a2) => a1.Id.CompareTo(a2.Id));
                        break;
                    case ProbeACLType.GAME:
                        aclList.Where(a => a.Name == "PRIVATE").FirstOrDefault().Description = "My Games";
                        aclList.Where(a => a.Name == "GLOBAL").FirstOrDefault().Description = "Shared Games";
                        aclList.Add(new ACL { Id = 0, Description = "All Games" });
                        aclList.Sort((a1, a2) => a1.Id.CompareTo(a2.Id));
                        break;
                }
                return Json(aclList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
