using Bookings.Api.Contract.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands;

namespace Bookings.API.Mappings
{
    public class QuestionAnswerRequestToNewQuestionAnswerMapper
    {

        public List<NewChecklistAnswer> MapQuestionAnswerToResponse(List<QuestionAnswerRequest> questionAnswerRequestList)
        {
            List<NewChecklistAnswer> checklistAnswers = new List<NewChecklistAnswer>();
            foreach (var checklistAnswer in questionAnswerRequestList)
            {
                checklistAnswers.Add(new NewChecklistAnswer() { Answer = checklistAnswer.Answer,  Notes = checklistAnswer.Notes, QuestionKey = checklistAnswer.QuestionKey });
            }

            return checklistAnswers;
        }
    }
}
