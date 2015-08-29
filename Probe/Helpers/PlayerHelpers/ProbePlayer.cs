using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProbeDAL.Models;

namespace Probe.Helpers.PlayerHelpers
{
    public class ProbePlayer
    {

        public Player Player { get; set; }

        //Constructor for a Game
        public ProbePlayer(Player player)
        {
            this.Player = player;
        }

        public string PlayerGameName {
            get
            {
                if (Player.FirstName != null && Player.LastName != null)
                {
                    return Player.FirstName + '-' + Player.LastName;
                }
                else if (Player.FirstName == null && Player.NickName != null && Player.LastName != null)
                {
                    return Player.NickName + '-' + Player.LastName;
                }
                else if (Player.FirstName != null && Player.LastName == null && Player.NickName != null)
                {
                    return Player.FirstName + '-' + Player.NickName;
                }
                else if (Player.FirstName != null && Player.LastName == null && Player.NickName == null)
                {
                    return Player.FirstName;
                }
                else if (Player.FirstName == null && Player.LastName != null && Player.NickName == null)
                {
                    return Player.LastName;
                }
                else if (Player.FirstName == null && Player.LastName == null && Player.NickName != null)
                {
                    return Player.NickName;
                }
                else if (Player.FirstName == null && Player.LastName == null && Player.NickName == null && Player.EmailAddr != null)
                {
                    return Player.EmailAddr; //last hope
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}