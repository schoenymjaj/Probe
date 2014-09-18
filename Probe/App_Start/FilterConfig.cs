using System;
using System.Web;
using System.Web.Mvc;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Logging;

namespace Probe
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
//            filters.Add(new HandleErrorAttribute());
            filters.Add(new ElmahHandleErrorAttribute());

            bool logMvcActionInd = false;
            if (System.Configuration.ConfigurationManager.AppSettings["LogMvcActions"] != null)
            {
                logMvcActionInd = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogMvcActions"]);
            }

            if (logMvcActionInd)
            {
                filters.Add(new MvcLogFilterAttribute());
            }

        }
    }
}
