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
                name: "Publish",
                url: "Games/Publish/{id}/{publishind}",
                defaults: new { controller = "Games", action = "Publish" }
            );

            routes.MapRoute(
                name: "About",
                url: "Home/About/{mobileind}",
                defaults: new { controller = "Home", action = "About", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetGamePlayerMatchMinMaxData",
                url: "Reports/GetGamePlayerMatchMinMaxData/{gameid}/{code}",
                defaults: new { controller = "Reports", action = "GetGamePlayerMatchMinMaxData" }
            );

            routes.MapRoute(
                name: "GamePlayerMatchMinMax",
                url: "Reports/GamePlayerMatchMinMax/{gameid}/{code}/{mobileind}",
                defaults: new { controller = "Reports", action = "GamePlayerMatchMinMax", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetPlayerMatchDetailData",
                url: "Reports/GetPlayerMatchDetailData/{gameid}/{code}/{playerid}/{matchedplayerId}",
                defaults: new { controller = "Reports", action = "GetPlayerMatchDetailData" }
            );


            routes.MapRoute(
                name: "PlayerMatchDetail",
                url: "Reports/PlayerMatchDetail/{gameid}/{code}/{playerid}/{matchedplayerId}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerMatchDetail", mobileind = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetPlayerMatchSummaryData",
                url: "Reports/GetPlayerMatchSummaryData/{gameid}/{code}/{playerid}",
                defaults: new { controller = "Reports", action = "GetPlayerMatchSummaryData" }
            );


            routes.MapRoute(
                name: "PlayerMatchSummary",
                url: "Reports/PlayerMatchSummary/{gameid}/{code}/{playerid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerMatchSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPlayerTestSummaryData",
                url: "Reports/GetPlayerTestSummaryData/{gameid}/{code}",
                defaults: new { controller = "Reports", action = "GetPlayerTestSummaryData" }
            );


            routes.MapRoute(
                name: "PlayerTestSummary",
                url: "Reports/PlayerTestSummary/{gameid}/{code}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerTestSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPlayerTestDetailData",
                url: "Reports/GetPlayerTestDetailData/{gameid}/{code}/{playerid}",
                defaults: new { controller = "Reports", action = "GetPlayerTestDetailData" }
            );


            routes.MapRoute(
                name: "PlayerTestDetail",
                url: "Reports/PlayerTestDetail/{gameid}/{code}/{playerid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerTestDetail", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GameLMSSummary",
                url: "Reports/GameLMSSummary/{gameid}/{code}/{mobileind}",
                defaults: new { controller = "Reports", action = "GameLMSSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PlayerLMSSummary",
                url: "Reports/PlayerLMSSummary/{gameid}/{code}/{playerstatusfilter}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerLMSSummary", mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PlayerLMSDetail",
                url: "Reports/PlayerLMSDetail/{gameid}/{code}/{playerid}/{mobileind}",
                defaults: new { controller = "Reports", action = "PlayerLMSDetail", playerstatusfilter = UrlParameter.Optional, mobileind = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
