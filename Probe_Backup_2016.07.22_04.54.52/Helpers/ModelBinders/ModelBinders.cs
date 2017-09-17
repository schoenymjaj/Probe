using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ProbeDAL.Models;
using Probe.Models.API;
using Probe.Models;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections;
using Probe.Helpers.Mics;

namespace Probe.Helpers.ModelBinders
{
    public class PlayerModelBinder : IModelBinder
    {
        /*
         * Updates: MNS 4-15-15 -   Added support for LastName, and Email. Also support the possibility that FirstName,
         *                          NickName, LastName, or Email may be missing. This is a valid use case.
         */ 
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            PlayerDTO playerDto = new PlayerDTO();
            bindingContext.Model = playerDto;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PlayerDTO));

            string ct = actionContext.Request.Content.ReadAsStringAsync().Result;

            JObject root = JObject.Parse(ct);
            playerDto.Id = (long)root["Id"];
            playerDto.GameCode = (string)root["GameCode"];
            playerDto.GameId = (long)root["GameId"];

            if (root["Sex"] != null) playerDto.Sex = (Person.SexType)((int)root["Sex"]);
            if (root["FirstName"] != null) playerDto.FirstName = root["FirstName"].ToString();
            if (root["NickName"] != null) playerDto.NickName = (string)root["NickName"];
            if (root["LastName"] != null) playerDto.LastName = (string)root["LastName"];
            if (root["Email"] != null) playerDto.EmailAddr = (string)root["Email"];

            try
            {
                if ((string)root["ClientVersion"] != null)
                {
                    playerDto.ClientVersion = (string)root["ClientVersion"];
                }
                else
                {
                    playerDto.ClientVersion = ProbeConstants.ClientVersionPostPlayerWithoutAnswers;
                }
            }
            catch
            {
                playerDto.ClientVersion = ProbeConstants.ClientVersionPostPlayerWithoutAnswers;
            }

            if (playerDto.ClientVersion != ProbeConstants.ClientVersionPostPlayerWithoutAnswers)
            {

                List<GameAnswerDTO> gaList = new List<GameAnswerDTO>();
                JArray gaArray = (JArray)root["GameAnswer"];
                for (int i = 0; i < gaArray.Count; i++) //loop through rows
                {
                    GameAnswerDTO ga = new GameAnswerDTO();
                    ga.Id = 0; //just a placeholder
                    ga.PlayerId = 0; //just a placeholder
                    ga.QuestionNbr = (int)gaArray[i]["QuestionNbr"];
                    ga.QuestionId = (long)gaArray[i]["QuestionId"];
                    ga.ChoiceId = (long)gaArray[i]["ChoiceId"];
                    gaList.Add(ga);
                }

                playerDto.GameAnswers = gaList;
            }

            return true;
        }
    }

    public class PlayerModelBinderProvider : ModelBinderProvider
    {
        public override IModelBinder GetBinder(System.Web.Http.HttpConfiguration configuration, Type modelType)
        {
            return new PlayerModelBinder();
        }
    }
}