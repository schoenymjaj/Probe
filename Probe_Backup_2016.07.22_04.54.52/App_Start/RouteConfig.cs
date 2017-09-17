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
                name: "Default",
                url: "{controller}/{action}/",
                defaults: new { controller = "Home", action = "Index" }
            );


            routes.MapRoute(
                name: "GameSchedules",
                url: "Games/GameSchedules/{gameid}",
                defaults: new { controller = "Games", action = "GameSchedules" }
            );

            routes.MapRoute(
                name: "GameConfigurations",
                url: "GameConfigurations/{action}/{gameid}",
                defaults: new { controller = "GameConfigurations", action = "Index" }
            );

            routes.MapRoute(
                name: "Choices",
                url: "Choices/{action}/{questionid}",
                defaults: new { controller = "Choices", action = "Index" }
            );

            routes.MapRoute(
                name: "Players",
                url: "Players/{action}/{gameid}",
                defaults: new { controller = "Players", action = "Index" }
            );

            routes.MapRoute(
            name: "GameQuestions",
            url: "GameQuestions/GameQuestions/{gameid}",
            defaults: new { controller = "GameQuestions", action = "GameQuestions"}
            );

            routes.MapRoute(
            name: "GameQuestions.GetGameQuestions",
            url: "GameQuestions/GetGameQuestions/{gameid}",
            defaults: new { controller = "GameQuestions", action = "GetGameQuestions" }
            );

            routes.MapRoute(
            name: "ChoiceQuestions.Get",
            url: "ChoiceQuestions/Get/{aclid}/{questionsearch}",
            defaults: new { controller = "ChoiceQuestions", action = "Get", questionsearch = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ChoiceQuestions.GetQuestions",
            url: "ChoiceQuestions/GetQuestions/{gameid}",
            defaults: new { controller = "ChoiceQuestions", action = "GetQuestions", gameid = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ChoiceQuestions.GetQuestionsForAutoComplete",
            url: "ChoiceQuestions/GetQuestionsForAutoComplete/{gameid}",
            defaults: new { controller = "ChoiceQuestions", action = "GetQuestionsForAutoComplete", gameid = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ChoiceQuestions.GetGameQuestions",
            url: "ChoiceQuestions/GetGameQuestions/{gameid}/{aclid}/{questionsearch}",
            defaults: new { controller = "ChoiceQuestions", action = "GetGameQuestions", questionsearch = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ChoiceQuestions.Clone",
            url: "ChoiceQuestions/Clone/{id}",
            defaults: new { controller = "ChoiceQuestions", action = "Clone" }
            );


            routes.MapRoute(
                name: "Publish",
                url: "Games/Publish/{id}/{publishind}",
                defaults: new { controller = "Games", action = "Publish" }
            );

            routes.MapRoute(
                name: "Clone",
                url: "Games/Clone/{id}",
                defaults: new { controller = "Games", action = "Clone" }
            );


            routes.MapRoute(
                name: "CloneToUser",
                url: "Games/CloneToUser/{id}/{userid}",
                defaults: new { controller = "Games", action = "CloneToUser" }
            );


            routes.MapRoute(
                name: "About",
                url: "Home/About/{id}",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
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
                name: "ACLs.Get",
                url: "ACLs/Get/{probeacltype}",
                defaults: new { controller = "ACLs", action = "Get" }
            );


            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Games", action = "Index", id = UrlParameter.Optional }
            //);

        }
    }
}
