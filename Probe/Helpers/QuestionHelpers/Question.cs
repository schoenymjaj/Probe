using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Helpers.Exceptions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using Probe.Helpers.Mics;
using Probe.Helpers.Validations;

namespace Probe.Helpers.QuestionHelpers
{
    public static class ProbeQuestion
    {
        /*
         * Will clone the question and all artifacts associated with that question. question/choicequestion,
         * choice records. Returns the cloned question Id.
         */
        public static long CloneQuestion(Controller controller, ProbeDataContext db, bool forQuestionInUse, long sourceQuestionId,
            ref Dictionary<long,long> choiceXreference )
        {

            //clone question
            ChoiceQuestion cqSource = db.ChoiceQuestion.Find(sourceQuestionId);

            string newName = cqSource.Name;
            if (!forQuestionInUse)
            {
                newName = GetClonedQuestionName(cqSource.Name);
            }

            ChoiceQuestion cqNew = new ChoiceQuestion
            {
                AspNetUsersId = cqSource.AspNetUsersId,
                QuestionTypeId = cqSource.QuestionTypeId,
                Name = newName,
                Text = cqSource.Text,
                OneChoice = cqSource.OneChoice,
                ACLId = cqSource.ACLId,
                UsedInGame = forQuestionInUse,
            };


            db.Question.Add(cqNew);
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new ChoiceQuestionId

            long clonedQuestionId = cqNew.Id;
            ChoiceQuestion cqOrg = db.ChoiceQuestion.Find(sourceQuestionId);
            //clone choices
            foreach (Choice c in cqOrg.Choices)
            {
                Choice newC = new Choice
                {
                    ChoiceQuestionId = clonedQuestionId,
                    Name = c.Name,
                    Text = c.Text,
                    OrderNbr = c.OrderNbr,
                    Correct = c.Correct 
                };

                db.Choice.Add(newC);
                db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null);

                //Here we populate the choice X reference table. Associate the old choice with the new choice. Somebody might need this down the road.
                choiceXreference.Add(c.Id, newC.Id);
            }

            return clonedQuestionId;
        }//CloneQuestions

        /*
         * Will delete the question and all artifacts associated with that question. question/choicequestion,
         * choice records.
         */
        public static void DeleteQuestion(Controller controller, ProbeDataContext db, long questionId)
        {

            ChoiceQuestion cq = db.ChoiceQuestion.Find(questionId);
            db.Choice.RemoveRange(cq.Choices);

            db.ChoiceQuestion.Remove(cq);
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null);

        }//DeleteQuestion

        private static string GetClonedQuestionName(string name)
        {
            string newName = "";
            string newNameLastBeforeToLong = "";
            int copyNumber = 0;
            string copyString = " COPY";

            /*
             * Let's just try and add the string "COPY" to the end of the current name to start with
             */
            newNameLastBeforeToLong = name.Trim();
            newName = name.Trim() + copyString;
            while (ProbeValidate.IsQuestionNameExistForLoggedInUser(newName) && (newName.Length < ProbeConstants.QuestionNameMaxChars))
            {
                newNameLastBeforeToLong = newName;
                newName = newName + copyString;
            };

            /*
             * If we get here and the new name doesn't exist and it's less than equal to the max characters,
             * THEN we have our new question name. Otherwise, we go to approach II
             */
            if (ProbeValidate.IsQuestionNameExistForLoggedInUser(newName) || (newName.Length > ProbeConstants.QuestionNameMaxChars))
            {
                newName = newNameLastBeforeToLong;
                do
                {
                    string copyNewString = " COPY";
                    if (copyNumber++ != 0)
                    {
                        copyNewString = copyString.Substring(0, 5 - copyNumber.ToString().Length) + copyNumber.ToString();
                    }

                    newName = newName.Substring(0, ProbeConstants.QuestionNameMaxChars - 5) + copyNewString;


                } while (ProbeValidate.IsQuestionNameExistForLoggedInUser(newName));


            }

            return newName;
        }

    }
}