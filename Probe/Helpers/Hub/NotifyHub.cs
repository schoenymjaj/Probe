using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Probe.Helpers.Hub
{
    [Authorize(RequireOutgoing = false)] 
    public class NotifyHub : Microsoft.AspNet.SignalR.Hub
    {
        public void GameChangeNotification(string userName)
        {
            //Clients.All.GameChangeNotification();
            //Clients.Group("XYZ").GameChangeNotification();

            Clients.User(userName).GameChangeNotification();
        }

        public void QuestionChangeNotification(string userName)
        {
            //Clients.All.QuestionChangeNotification();
            //Clients.Group("XYZ").QuestionChangeNotification();

            Clients.User(userName).QuestionChangeNotification();
        }

        public void ChoiceChangeNotification(string userName)
        {
            //Clients.All.ChoiceChangeNotification();
            //Clients.Group("XYZ").ChoiceChangeNotification();

            Clients.User(userName).ChoiceChangeNotification();
        }

        //public override System.Threading.Tasks.Task OnConnected()
        //{

        //    if (Context.User.Identity.IsAuthenticated)
        //    {
        //        Groups.Add(Context.ConnectionId, Context.User.Identity.Name);
        //    }

        //    return base.OnConnected();
        //}

    }

}