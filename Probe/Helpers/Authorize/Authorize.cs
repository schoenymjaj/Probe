using System;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;


namespace Probe.Helpers.Authorize
{
    public class RoleAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //currently all roles are filtered through.
            return true;
        }
    }
}