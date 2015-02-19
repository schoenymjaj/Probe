using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
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
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            PlayerDTO playerDto = new PlayerDTO();
            bindingContext.Model = playerDto;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PlayerDTO));

            string ct = actionContext.Request.Content.ReadAsStringAsync().Result;

            //THIS DESERIALIZATION ONLY GETS THE FIRST LEVEL OF DATA. IT DOES NOT GET THE GAMEPLAYANSWER NESTED LIST
            //byte[] byteArray = Encoding.ASCII.GetBytes(ct);
            //MemoryStream stream = new MemoryStream(byteArray);
            //stream.Position = 0;
            //playerDto = (PlayerDTO)ser.ReadObject(stream);

            JObject root = JObject.Parse(ct);
            playerDto.Id = 0; //just a placeholder
            playerDto.FirstName = root["FirstName"].ToString();
            playerDto.GameCode = (string)root["GameCode"];
            playerDto.GamePlayId = (long)root["GamePlayId"];
            playerDto.NickName = (string)root["NickName"];
            playerDto.Sex = (Person.SexType)((int)root["Sex"]);

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

                List<GamePlayAnswer> gpaList = new List<GamePlayAnswer>();
                JArray gpaArray = (JArray)root["GamePlayAnswer"];
                for (int i = 0; i < gpaArray.Count; i++) //loop through rows
                {
                    GamePlayAnswer gpa = new GamePlayAnswer();
                    gpa.Id = 0; //just a placeholder
                    gpa.PlayerId = 0; //just a placeholder
                    gpa.ChoiceId = (long)gpaArray[i]["ChoiceId"];
                    gpaList.Add(gpa);
                }

                playerDto.GamePlayAnswers = gpaList;
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