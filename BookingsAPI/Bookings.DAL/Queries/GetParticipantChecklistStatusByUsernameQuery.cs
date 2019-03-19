using Bookings.DAL.Queries.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Bookings.DAL.Queries
{
    public class GetParticipantChecklistStatusByUsernameQuery : IQuery
    {
        public GetParticipantChecklistStatusByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    //public class GetParticipantChecklistStatusByUsernameQueryHandler : IQueryHandler<GetAllParticipantsChecklistsQuery, List<Participant>>
    //{
    //    private readonly BookingsDbContext _context;

    //    public GetParticipantChecklistStatusByUsernameQueryHandler(BookingsDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<bool> Handle(GetParticipantChecklistStatusByUsernameQuery query)
    //    {

    //        var person = await _context.Persons
    //                  .SingleOrDefaultAsync(x => x.Username == query.Username);
            
    //        //List<Participant> participants = new List<Participant>();
    //        //var hearings = await _context.VideoHearings
    //        //    .Include("HearingCases.Case")
    //        //    .Include("Participants.Person")
    //        //    .Include("Participants.HearingRole.UserRole")
    //        //    .Include("Participants.CaseRole")
    //        //    .Include("Participants.ChecklistAnswers")
    //        //    .Include("Participants.ChecklistAnswers.Question").ToListAsync();

    //        //foreach (var hearing in hearings)
    //        //{
    //        //    participants.AddRange(hearing.GetParticipants());
    //        //}

    //        //return participants;

    //        return false;
    //    }
    //}
}
