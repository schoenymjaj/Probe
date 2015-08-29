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
    public class AdminController : Controller
    {

        /*
         * BELOW - THIS ALL WORKED, BUT IT WAS A ONE TIME THING TO ADD THE NECESSARY ROLE AND USER ROLE
         */
        // GET: Admin
        //[Authorize(Roles = "Admin")]
        //public ActionResult Index()
        //{
        //    HACK TO BE RUN ONCE.
        //    add Admin Role and assign mns@productivityedge.com user to that admin role.

        //    ApplicationDbContext dbIdentity = new ApplicationDbContext();

        //    var um = new UserManager<ApplicationUser>(
        //            new UserStore<ApplicationUser>(dbIdentity));

        //    string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    bool currentUserinRole = um.IsInRole(currentUserId, "Admin");

        //    IdentityResult ir;
        //    var rm = new RoleManager<IdentityRole>
        //        (new RoleStore<IdentityRole>(dbIdentity));

        //    if (!rm.RoleExists("Admin")) {

        //        ir = rm.Create(new IdentityRole("Admin"));

        //        ApplicationUser userAdmin = um.Users.Where(u => u.UserName == "mns@productivityedge.com").Single();

        //        if (!um.IsInRole(userAdmin.Id, "Admin")) {

        //            um.AddToRole(userAdmin.Id,"Admin");

        //            bool userAdmininRole = um.IsInRole(userAdmin.Id, "Admin");

        //        }

        //    }

        //    return View();
        //}
    }
}