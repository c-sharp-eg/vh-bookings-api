using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    public class HearingParticipantCheckListResponse
    {
        public Guid HearingId { get; set; }

        public Guid ParticipantId { get; set; }

        /// <summary>
        /// The participants title (i.e. Mr, Ms)
        /// </summary>
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>The participant role</summary>
        /// <remarks>Important since the questions are determined per role</remarks>
        public string Role { get; set; }

        /// <summary>
        /// The date and time the checklist was submitted 
        /// </summary>
        public DateTime CompletedDate { get; set; }

        /// <summary>
        /// Checklist answers submitted
        /// </summary>
        public List<QuestionAnswerResponse> QuestionAnswerResponses { get; set; }

        /// <summary>
        /// Landline number to the participant
        /// </summary>
        public string Landline { get; set; }

        /// <summary>
        /// Mobile phone number to the participant
        /// </summary>
        public string Mobile { get; set; }
    }
}
