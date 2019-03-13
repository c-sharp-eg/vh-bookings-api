using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Requests
{
    public class AddPreHearingChecklistRequest
    {
        public List<QuestionAnswerRequest> QuestionAnswers { get; set; }
    }
}
