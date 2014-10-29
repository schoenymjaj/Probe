using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Probe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "About",
                url: "Home/About/{mobileind}",
                defaults: new { controller = "Home", action = "About", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetGamePlayerMatchMinMaxData",
                url: "Reports/GetGamePlayerMatchMinMaxData/{gameplayid}",
                defaults: new { controller = "Reports", action = "GetGamePlayerMatchMinMaxData" }
            );

            routes.MapRoute(
                name: "GamePlayerMatchMinMax",
                url: "Reports/GamePlayerMatchMinMax/{gameplayid}/{mobileind}",
                defaults: new { controller = "Reports", action = "GamePlayerMatchMinMax", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetPlayerMatchDetailData",
                url: "Reports/GetPlayerMatchDetailData/{gameplayid}/{playerid}/{matchedplayerId}",
                defaults: new { controller = "Reports", action = "GetPlayerMatchDetailData" }
            );


            routes.MapRoute(
                name: "PlayerMatchDetail",
                url: "Reports/PlayerMatchDetail/{gameplayid}/{playerid}/{matchedplayerId}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerMatchDetail", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetPlayerMatchSummaryData",
                url: "Reports/GetPlayerMatchSummaryData/{gameplayid}/{playerid}",
                defaults: new { controller = "Reports", action = "GetPlayerMatchSummaryData" }
            );


            routes.MapRoute(
                name: "PlayerMatchSummary",
                url: "Reports/PlayerMatchSummary/{gameplayid}/{playerid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerMatchSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPlayerTestSummaryData",
                url: "Reports/GetPlayerTestSummaryData/{gameplayid}",
                defaults: new { controller = "Reports", action = "GetPlayerTestSummaryData" }
            );


            routes.MapRoute(
                name: "PlayerTestSummary",
                url: "Reports/PlayerTestSummary/{gameplayid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerTestSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPlayerTestDetailData",
                url: "Reports/GetPlayerTestDetailData/{gameplayid}/{playerid}",
                defaults: new { controller = "Reports", action = "GetPlayerTestDetailData" }
            );


            routes.MapRoute(
                name: "PlayerTestDetail",
                url: "Reports/PlayerTestDetail/{gameplayid}/{playerid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerTestDetail", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
