using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetParticipantsInHearingQueryHandlerTests : DatabaseTestsBase
    {
        private GetParticipantsInHearingQueryHandler _handler;
        private Guid _newHearingId;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetParticipantsInHearingQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public async Task should_get_participants_in_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var participants = await _handler.Handle(new GetParticipantsInHearingQuery(_newHearingId));
            participants.Count.Should().Be(seededHearing.GetParticipants().Count);

            foreach (var participant in participants)
            {
                participant.DisplayName.Should().NotBeNullOrEmpty();

                participant.HearingRole.Should().NotBeNull();
                participant.HearingRole.Name.Should().NotBeNull();

                participant.CaseRole.Should().NotBeNull();
                participant.CaseRole.Name.Should().NotBeNull();
                var person = participant.Person;
                var existingPerson = seededHearing.GetParticipants().First(x => x.Person.Username == person.Username)
                    .Person;
                person.Should().BeEquivalentTo(existingPerson);

            }
        }
        
        [Test]
        public void should_not_get_participants_in_hearing_that_does_not_exist()
        {
            Assert.ThrowsAsync<HearingNotFoundException>(() => _handler.Handle(new GetParticipantsInHearingQuery(Guid.Empty)));
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