using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

namespace Probe.Helpers.Mics
{
    public static class ProbeConstants
    {
        public const string ServerVersion = "1.4";
        public const long NoPrimaryKeyId = -1;
        public const int ValueIntNone = -1;
        public const string ValueStringNone = "";

        public const string ClientVersionPostPlayerWithoutAnswers = "1.0";
        public const int GameNameMaxChars = 60;
        public const int GameDescriptionMaxChars = 300;
        public const int QuestionNameMaxChars = 60;
        public const int QuestionDescriptionMaxChars = 300;
        public const int GameCodeMaxChars = 60;
        public const int ChoiceNameMaxChars = 60;
        public const int ChoiceDescriptionMaxChars = 300;

        public const int MSG_NoError = 0;
        public const int MSG_GameNotActive = 2;
        public const int MSG_PlayerDupInGame = 3;
        public const int MSG_PlayerFirstNameInvalid = 4;
        public const int MSG_PlayerNickNameInvalid = 5;
        public const int MSG_SubmissionMissingAnswers = 6;
        public const int MSG_SubmissionInvalidAnswers = 7;
        public const int MSG_PlayerNameInvalid = 8;
        public const int MSG_PlayerLastNameInvalid = 9;
        public const int MSG_SubmissionNotOntime = 10;
        public const int MSG_GamePlayerInActive = 11;
        public const int MSG_SubmissionTooEarly = 12;
        public const int MSG_NewInCommonVersionMustBeInstalled = 13;
        public const int MSG_GameHasNoQuestions = 14;
        public const int MSG_GameHasPlayers = 15;
        public const int MSG_GameStartGTEndDate = 16;
        public const int MSG_GameEndDateIsPassed = 17;
        public const int MSG_GameInSuspendMode = 18;
        public const int MSG_GameCloneSuccessful = 19;
        public const int MSG_GameDoesNotExist = 20;
        public const int MSG_GamePublishSuccessful = 21;
        public const int MSG_GameUnpublishSuccessful = 22;
        public const int MSG_QuestionCloneSuccessful = 23;
        public const int MSG_UnsuccessfulOperation = 24;
        public const int MSG_GameDeleteSuccessful = 25;

        public const string MatchGameType = "Match";
        public const string TestGameType = "Test";
        public const string LMSGameType = "Last Man Standing";

        public const string DateTimeStandardFormat = "M/d/yy h:mm tt";

        public const string ProbeAuthorizedStartUrl = "~/Games/Index";

        public const int MaxAboutSections = 10;
    }

    public enum PlayersFilterType
    {
        ALL = 0,
        ACTIVE = 1,
        INACTIVE = 2
    }

    public enum GameStatusType
    {
        UNKNOWN = 0,
        NOTSTARTED = 1,
        STARTEDNOQUESTIONPASSED = 2,
        ACTIVE = 3,
        SUSPENDED = 4,
        COMPLETED = 5
    }

    //Didn't use UPPER CASE because these enumerations are being displayed
    public enum MessageType
    {
        Informational = 0,
        Warning = 1,
        Error = 2,
    }



    public static class EnumHelper
    {
        // Get the value of the description attribute if the   
        // enum has one, otherwise use the value.  
        public static string GetDescription<TEnum>(this TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// Build a select list for an enum
        /// </summary>
        public static SelectList SelectListFor<T>() where T : struct
        {
            Type t = typeof(T);
            return !t.IsEnum ? null
                             : new SelectList(BuildSelectListItems(t), "Value", "Text");
        }

        /// <summary>
        /// Build a select list for an enum with a particular value selected 
        /// </summary>
        public static SelectList SelectListFor<T>(T selected) where T : struct
        {
            Type t = typeof(T);
            return !t.IsEnum ? null
                             : new SelectList(BuildSelectListItems(t), "Value", "Text", selected.ToString());
        }

        private static IEnumerable<SelectListItem> BuildSelectListItems(Type t)
        {
            return Enum.GetValues(t)
                       .Cast<Enum>()
                       .Select(e => new SelectListItem { Value = e.ToString(), Text = e.GetDescription() });
        }
    }

}