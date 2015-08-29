using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Probe.Helpers.Hub;
using Microsoft.AspNet.SignalR;

namespace Probe.Helpers.Mics
{
    public class NotifyProbe
    {
        static public void NotifyGameChanged(string userName)
        {
            //Triggers a SignalR message that a game has changed. 
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            context.Clients.User(userName).GameChangeNotification();
        }

        static public void NotifyQuestionChanged(string userName)
        {
            //Triggers a SignalR message that a game has changed. 
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            context.Clients.User(userName).QuestionChangeNotification();
        }

        static public void NotifyChoiceChanged(string userName)
        {
            //Triggers a SignalR message that a game has changed. 
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            context.Clients.User(userName).ChoiceChangeNotification();
        }

    }
}