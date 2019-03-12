using Bookings.Api.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.API.Mappings
{
    public class QuestionAnswerToResponseMapper
    {
        public List<QuestionAnswerResponse> MapQuestionAnswerToResponse(List<Domain.ChecklistAnswer> checklistAnswers)
        {
            List<QuestionAnswerResponse> questionAnswerResponses = new List<QuestionAnswerResponse>();
            foreach(var checklistAnswer in checklistAnswers)
            {
                questionAnswerResponses.Add(new QuestionAnswerResponse() { Answer = checklistAnswer.Answer, CreatedAt=checklistAnswer.CreatedAt, Notes=checklistAnswer.Notes, QuestionKey= checklistAnswer.Question.Key });
            }

            return questionAnswerResponses;
        }
    }
}
