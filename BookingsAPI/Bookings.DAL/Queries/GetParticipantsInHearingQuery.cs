using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetParticipantsInHearingQuery : IQuery
    {
        public Guid HearingId { get; set; }

        public GetParticipantsInHearingQuery(Guid hearingId)
        {
            HearingId = hearingId;
        }
    }
    
    public class GetParticipantsInHearingQueryHandler : IQueryHandler<GetParticipantsInHearingQuery, List<Participant>>
    {
        private readonly BookingsDbContext _context;

        public GetParticipantsInHearingQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Participant>> Handle(GetParticipantsInHearingQuery query)
        {
            var hearing = await _context.VideoHearings
                .Include("Participants.Person.Address")
                .Include("Participants.Person.Organisation")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .SingleOrDefaultAsync(x => x.Id == query.HearingId);
            if (hearing == null)
            {
                throw new HearingNotFoundException(query.HearingId);
            }    
            return hearing.GetParticipants().ToList();
        }
    }
}