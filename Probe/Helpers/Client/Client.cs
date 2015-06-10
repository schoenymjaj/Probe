using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Probe.Helpers.Client
{
    public class ClientTimeZoneAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["timeZoneOffset"];

            Int16 n = 0;

            var tzo = (cookie != null && Int16.TryParse(cookie.Value, out n) ? n : 0);

            filterContext.HttpContext.Session["tzo"] = tzo;

            base.OnActionExecuting(filterContext);
        }
    }
}