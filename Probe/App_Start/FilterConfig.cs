﻿using System;
using System.Web;
using System.Web.Mvc;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Logging;
using Probe.Helpers.Authorize;
using Probe.Helpers.Redirecting;
using Probe.Helpers.Client;

namespace Probe
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            //filters.Add(new RoleAuthorizeAttribute());
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ElmahHandleErrorAttribute());
            //filters.Add(new RedirectFilterAttribute());

            //Get client timezone from cookies that its leaving
            filters.Add(new ClientTimeZoneAttribute());

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
