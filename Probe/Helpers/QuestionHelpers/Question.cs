using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using Probe.DAL;
using Probe.Models;
using Probe.Helpers.Exceptions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Probe.Helpers.QuestionHelpers
{
    public static class ProbeQuestion
    {
        public static long CloneQuestion(Controller controller, ProbeDataContext db, long gameId, long sourceQuestionId)
        {

            //clone question
            ChoiceQuestion cqClone = db.ChoiceQuestion.Find(sourceQuestionId);
            cqClone.UsedInGame = true;
            db.Question.Add(cqClone);
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new ChoiceQuestionId

            long clonedQuestionId = cqClone.Id;
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
            }

            return clonedQuestionId;
        }//CloneQuestions

        public static void DeleteQuestion(Controller controller, ProbeDataContext db, long questionId)
        {

            ChoiceQuestion cq = db.ChoiceQuestion.Find(questionId);
            db.Choice.RemoveRange(cq.Choices);

            db.ChoiceQuestion.Remove(cq);
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null);

        }


    }
}