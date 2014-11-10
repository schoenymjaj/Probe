using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;
using Elmah;
using System.Web.Script.Serialization;
using Probe.Helpers.Exceptions;


namespace Probe.Helpers.Redirecting
{

    /*
     * Class to implement Redirect on every http request (initialized in the FilterConfig class). If the site
     * has been accessed by a naked domain name then we want to redirect to a www prefix. This way; we won't 
     * have to buy another SSL cert for the naked domain (in-common-app.com)
     */
    public class RedirectFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = System.Web.HttpContext.Current.Request.Url.OriginalString;
            var baseUrl = System.Web.HttpContext.Current.Request.Url.Authority;
            if (url.ToLower().Contains("in-common-app.com") && !url.ToLower().Contains("www.in-common-app.com"))
            {
                Log(filterContext);
                filterContext.Result = new RedirectResult(url.Replace("in-common-app.com", "www.in-common-app.com"));
                return;
            }
        }

        private void Log(ActionExecutingContext filterContext)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new ActionException("Redirecting:" + filterContext.HttpContext.Request.Url.ToString()));

        }

    }

}

