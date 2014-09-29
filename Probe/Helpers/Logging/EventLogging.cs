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


namespace Probe.Helpers.Logging
{

    /*
     * Class to implement MVC Controller logging on every http request (initialized in the FilterConfig class)
     */
    public class MvcLogFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //don't want to record controller action for error. driven by not wanting to log 
            //javascript log event
            if (filterContext.Controller.ToString() != "Probe.Controllers.ErrorController") 
            {
                Log(filterContext);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterExecutedContext)
        {
            //don't want to record controller action for error. driven by not wanting to log 
            //javascript log event
            if (filterExecutedContext.Controller.ToString() != "Probe.Controllers.ErrorController")
            {
                LogActionExecuted(filterExecutedContext);
            }
        }

        private void Log(ActionExecutingContext filterContext)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new ActionException("Executing:" + filterContext.HttpContext.Request.Url.ToString()));

        }

        private void LogActionExecuted(ActionExecutedContext filterExecutedContext)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new ActionException("Executed:" + filterExecutedContext.HttpContext.Request.Url.ToString()));
        }

    }

    /*
     * Class to implement Web Api logging on every http request (initialized in the WebApiConfig class)
     */
    public class WebApiLogFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Log(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            LogActionExecuted(actionExecutedContext);
        }



        private void Log(HttpActionContext actionContext)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            object firstObject = actionContext.ActionArguments.FirstOrDefault().Value;
            string strValue = (firstObject != null) ? serializer.Serialize(firstObject) : "No argument data";

            Elmah.ErrorSignal.FromCurrentContext().Raise(new ActionException("Executing:" + actionContext.Request.RequestUri.ToString() + Environment.NewLine + "Data:" + strValue ));
        }

        private void LogActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Elmah.ErrorSignal.FromCurrentContext().Raise(new ActionException("Executed:" + actionExecutedContext.Request.RequestUri.ToString()));
        }

    }

}