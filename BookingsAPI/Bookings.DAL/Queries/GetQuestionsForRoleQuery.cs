using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bookings.Domain.Participants;

namespace Bookings.DAL.Queries
{
    public class GetQuestionsForRoleQuery : IQuery
    {
        public Participant Participant { get; set; }

        public GetQuestionsForRoleQuery(Participant participant)
        {
            Participant = participant;
        }
    }

    public class GetQuestionsForRoleQueryHandler : IQueryHandler<GetQuestionsForRoleQuery, List<ChecklistQuestion>>
    {
        private readonly BookingsDbContext _context;

        public GetQuestionsForRoleQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChecklistQuestion>> Handle(GetQuestionsForRoleQuery query)
        {

            List<ChecklistQuestion> checklistQuestions = new List<ChecklistQuestion>();
            var questions = await _context.ChecklistQuestions
                .Include("UserRole")
                 .ToListAsync();
            checklistQuestions = questions.Where(x => x.UserRole.Name.ToLower().Equals(query.Participant.HearingRole.UserRole.Name.ToLower())).ToList();
            return checklistQuestions;

        }
    }
}
