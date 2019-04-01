using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    /// <summary>
    /// Detailed information of a hearing
    /// </summary>
    public class HearingDetailsResponse
    {
        /// <summary>
        ///     Hearing Id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        ///     The date and time for a hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        
        /// <summary>
        ///     The duration of a hearing (number of minutes)
        /// </summary>
        public int ScheduledDuration { get; set; }
        
        /// <summary>
        ///     The name of the hearing venue
        /// </summary>
        public string HearingVenueName { get; set; }
        
        /// <summary>
        ///     The name of the case type
        /// </summary>
        public string CaseTypeName { get; set; }
        
        /// <summary>
        ///     The name of the hearing type
        /// </summary>
        public string HearingTypeName { get; set; }
        
        /// <summary>
        ///     List of cases associated to the hearing
        /// </summary>
        public List<CaseResponse> Cases { get; set; }
        
        /// <summary>
        ///     List of participants in hearing
        /// </summary>
        public List<ParticipantResponse> Participants { get; set; }

        /// <summary>
        ///     The hearing room name at the hearing venue
        /// </summary>
        public string HearingRoomName { get; set; }

        /// <summary>
        ///     Any other information about the hearing
        /// </summary>
        public string OtherInformation { get; set; }

        /// <summary>
        ///     The VH admin username that created the hearing
        /// </summary>
        public string CreatedBy { get; set; }
    }
}