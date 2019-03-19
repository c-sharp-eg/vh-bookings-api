using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetAllParticipantsChecklistsQuery : IQuery
    {
    }

    public class GetAllParticipantsChecklistsQueryHandler : IQueryHandler<GetAllParticipantsChecklistsQuery, List<Participant>>
    {
        private readonly BookingsDbContext _context;

        public GetAllParticipantsChecklistsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Participant>> Handle(GetAllParticipantsChecklistsQuery query)
        {

            List<Participant> participants = new List<Participant>();
            var hearings = await _context.VideoHearings
                .Include("HearingCases.Case")
                .Include("Participants.Person")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .Include("Participants.ChecklistAnswers")
                .Include("Participants.ChecklistAnswers.Question").ToListAsync();

            foreach(var hearing in hearings)
            {
                participants.AddRange(hearing.GetParticipants());
            }

            return participants;

        }
    }
}
