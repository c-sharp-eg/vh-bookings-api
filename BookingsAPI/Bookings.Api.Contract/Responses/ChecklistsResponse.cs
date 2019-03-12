using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    public class ChecklistsResponse : PagedResponse
    {
        /// <summary>
        /// Gets or sets check list for hearing and participant.
        /// </summary>
        public List<HearingParticipantCheckListResponse> Checklists { get; set; }

        /// <summary>
        /// A list of hearings referenced by checklist responses in <see cref="Checklists"/>.
        /// </summary>
        public List<ChecklistsHearingResponse> Hearings { get; set; }
    }
}
