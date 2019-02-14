using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class SaveVideoHearingCommandTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _queryHandler;
        private SaveVideoHearingCommandHandler _commandHandler;
        private Guid _newHearingId;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _queryHandler = new GetHearingByIdQueryHandler(context);
            _commandHandler = new SaveVideoHearingCommandHandler(context);
        }

        [Test]
        public async Task should_be_able_to_save_video_hearing_to_database()
        {
            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);
            
            var claimantCaseRoles = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var defendantCaseRoles = caseType.CaseRoles.First(x => x.Name == "Defendant");
            
            var claimantLipHearingRole = claimantCaseRoles.HearingRoles.First(x => x.Name == "Claimant LIP");
            var defendantSolicitorHearingRole = defendantCaseRoles.HearingRoles.First(x => x.Name == "Solicitor");
            
            var hearingTypeName = "Application to Set Judgment Aside";
            var hearingType = caseType.HearingTypes.First(x => x.Name == hearingTypeName);
      
            var venues = new RefDataBuilder().HearingVenues;
            
            var person1 = new PersonBuilder(true).Build();
            var claimantLipParticipant = new Builder(BuilderSettings).CreateNew<Individual>().WithFactory(() => 
                new Individual(person1, claimantLipHearingRole, claimantCaseRoles)
            ).Build();
            
            var person2 = new PersonBuilder(true).Build();
            var defendantSolicitorParticipant = new Builder(BuilderSettings).CreateNew<Representative>().WithFactory(() => 
                new Representative(person2, defendantSolicitorHearingRole, defendantCaseRoles)
                ).Build();
            
            var scheduledDate = DateTime.Today.AddHours(10).AddMinutes(30);
            var duration = 45;
            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First())
            {
                CreatedBy = "test@integration.com"
            };
            videoHearing.AddParticipants(new Participant[]{claimantLipParticipant, defendantSolicitorParticipant});
  
            videoHearing.AddCase("1234567890", "Test Case", true);
            videoHearing.AddCase("1234567891", "Test Case2", false);

            await _commandHandler.Handle(new SaveVideoHearingCommand(videoHearing));
            videoHearing.Id.Should().NotBeEmpty();
            _newHearingId = videoHearing.Id;
            
            var returnedVideoHearing = _queryHandler.Handle(new GetHearingByIdQuery(videoHearing.Id));

            returnedVideoHearing.Should().NotBeNull();
            
            returnedVideoHearing.CaseType.Should().NotBeNull();
            returnedVideoHearing.HearingVenue.Should().NotBeNull();
            returnedVideoHearing.HearingType.Should().NotBeNull();
            
            var participants = returnedVideoHearing.GetParticipants();
            participants.Any().Should().BeTrue();
            participants.Single(x => x.GetType() == typeof(Individual)).Should().NotBeNull();
            participants.Single(x => x.GetType() == typeof(Representative)).Should().NotBeNull();

            returnedVideoHearing.GetPersons().Count.Should().Be(participants.Count);
            var cases = returnedVideoHearing.GetCases();
            returnedVideoHearing.GetCases().Any(x => x.IsLeadCase).Should().BeTrue();
            cases.Count.Should().Be(2);
        }
        
        private CaseType GetCaseTypeFromDb(string caseTypeName)
        {
            CaseType caseType;
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                caseType = db.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .First(x => x.Name == caseTypeName);
            }

            return caseType;
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}