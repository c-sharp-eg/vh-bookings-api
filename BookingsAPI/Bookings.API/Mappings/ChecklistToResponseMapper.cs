using Bookings.Api.Contract.Responses;
using Bookings.Domain.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.API.Mappings
{
    public class ChecklistToResponseMapper
    {
        public ChecklistsResponse MapChecklistToResponse(List<Participant> participants)
        {
            return new ChecklistsResponse
            {
                Checklists = participants.Select(MapParticipantChecklist).ToList()
            };
        }

        private HearingParticipantCheckListResponse MapParticipantChecklist(Participant hearingParticipant)
        {
            var checklistAnswers = hearingParticipant.GetChecklistAnswers().OrderByDescending(c => c.CreatedAt).ToList();
            var participant = hearingParticipant.Person;
            var answers = new QuestionAnswerToResponseMapper().MapQuestionAnswerToResponse(checklistAnswers);

            return new HearingParticipantCheckListResponse
            {
                Title = participant.Title,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Landline = participant.TelephoneNumber,
                Role = hearingParticipant.HearingRole.UserRole.Name.ToString(),
                //Mobile = participant.Mobile,
                HearingId = hearingParticipant.HearingId,
                ParticipantId = participant.Id,
                CompletedDate = checklistAnswers.Any() ? checklistAnswers.Max(answer => answer.CreatedAt) : DateTime.MinValue,
                QuestionAnswerResponses = answers
            };
        }
    }
}
