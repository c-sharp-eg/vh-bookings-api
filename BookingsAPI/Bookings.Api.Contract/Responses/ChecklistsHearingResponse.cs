using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    /// <summary>
    /// Hearing information for participant checklists
    /// </summary>
    public class ChecklistsHearingResponse
    {
        public long HearingId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public string Status { get; set; }

        public List<CaseResponse> Cases { get; set; }
    }
}
