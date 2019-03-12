using Bookings.Api.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.API.Mappings
{
    public class ChecklistHearingToResponseMapper
    {
        public ChecklistsHearingResponse MapQuestionAnswerToResponse(Guid hearingId)
        {
            return new ChecklistsHearingResponse()
            {
                HearingId = hearingId
            };
        }
    }
}
