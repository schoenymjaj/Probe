using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Probe.Helpers.Logging;

namespace Probe
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                //routeTemplate: "api/{controller}/{id}",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            /*
             * Setup Web API logging for every request if the appropriate App Setting LogWebApiActions is set to TRUE
             */
            bool logWebApiActionInd = false;
            if (System.Configuration.ConfigurationManager.AppSettings["LogWebApiActions"] != null)
            {
                logWebApiActionInd = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogWebApiActions"]);
            }

            if (logWebApiActionInd)
            {
                var filters = System.Web.Http.GlobalConfiguration.Configuration.Filters;
                filters.Clear();
                filters.Add(new WebApiLogFilterAttribute());
            }

        }
    }
}
