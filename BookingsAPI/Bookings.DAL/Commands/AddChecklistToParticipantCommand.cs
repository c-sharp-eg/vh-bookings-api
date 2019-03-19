using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;


namespace Bookings.DAL.Commands
{
    public class NewChecklistAnswer
    {
        public string QuestionKey { get; set; }
        public string Answer { get; set; }
        public string Notes { get; set; }
    }

    public class AddChecklistToParticipantCommand:ICommand
    {
        public AddChecklistToParticipantCommand(Guid hearingId,Guid participantId, List<NewChecklistAnswer> questionAnswersList)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
            QuestionAnswersList = questionAnswersList;
        }

        public List<ChecklistQuestion> QuestionsForRole { get; set; }
        public List<NewChecklistAnswer> QuestionAnswersList { get; set; }
        public Guid HearingId { get; set; }
        public Guid ParticipantId { get; set; }
    }

    public class AddChecklistToParticipantCommandHandler : ICommandHandler<AddChecklistToParticipantCommand>
    {
        private readonly BookingsDbContext _context;

        public AddChecklistToParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AddChecklistToParticipantCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include("Participants.Person")
                .Include("HearingCases.Case")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var participants = hearing.GetParticipants().ToList();

            var participant = participants.FirstOrDefault(x => x.Id == command.ParticipantId);

            if (participant == null)
            {
                throw new HearingNotFoundException(command.ParticipantId);
            }

            
            var questions = await _context.ChecklistQuestions
                .Include("UserRole")
                 .ToListAsync();

            var checklistQuestionsForRole = questions.Where(x => x.UserRole.Name.ToLower().Equals(participant.HearingRole.UserRole.Name.ToLower())).ToList();

            if (!checklistQuestionsForRole.Any())
            {
                throw new CheckListQuestionsForRoleNotFoundException(participant.HearingRole.UserRole.Name);
            }

            var newChecklist = Checklist.New(participant, checklistQuestionsForRole);

            foreach (var questionAnswer in command.QuestionAnswersList)
                newChecklist.Answer(questionAnswer.QuestionKey, questionAnswer.Answer, questionAnswer.Notes);

            participant.UpdateChecklist(newChecklist);

            await _context.SaveChangesAsync();
        }


        }
    }
