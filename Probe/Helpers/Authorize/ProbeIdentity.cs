using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Probe.Helpers.Authorize
{
    public class ProbeIdentity
    {
        private ApplicationDbContext dbIdentity = new ApplicationDbContext();


        public ProbeIdentity()
        {
        }

        /*
         * Get the User Name (login username) from the User ID (ASPNET Login ID)
         */
        public string GetUserNameFromUserId(string userId)
        {
            var um = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(dbIdentity));

            ApplicationUser au = um.FindById(userId);

            return au.UserName;
        }




    }
}