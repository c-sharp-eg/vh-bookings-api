using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    public class ParticipantChecklistResponse
    {
        /// <summary>
        /// The responses to checklist question the participant has submitted
        /// </summary>
        public List<QuestionAnswerResponse> QuestionAnswerResponses { get; set; }
    }
}
